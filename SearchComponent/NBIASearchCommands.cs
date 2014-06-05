﻿#region License

// Copyright (c) 2007 - 2014, Northwestern University, Vladimir Kleper, Skip Talbot
// and Pattanasak Mongkolwat.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//   Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
//   Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
//   Neither the name of the National Cancer Institute nor Northwestern University
//   nor the names of its contributors may be used to endorse or promote products
//   derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Data;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using DataServiceUtil;
using NBIAService;

namespace SearchComponent
{
	internal partial class NBIASearchCoordinator
	{
		private class QueryForStudiesCommand : SearchCommand
		{
			private readonly NBIAQueryParameters _queryParameters;

			public QueryForStudiesCommand(NBIAQueryParameters queryParameters)
			{
				_queryParameters = queryParameters;
			}

			public override void Execute()
			{
				List<NBIASearchResult> results = new List<NBIASearchResult>();

				// DO study search
				NBIAStudy studyService = new NBIAStudy();

				try
				{
					DataTable dtStudies = studyService.getStudyInfo(_queryParameters, SearchSettings.Default.NBIADataServiceUrl);
					if (dtStudies != null)
					{
						foreach (DataRow studyUid in dtStudies.Rows)
						{
							if (base.CancelRequested)
								break;

							NBIASearchResult result = new NBIASearchResult();
							result.Study.StudyInstanceUid = studyUid["studyInstanceUID"].ToString();
							results.Add(result);
						}
					}
				}
				catch (GridServicerException ex)
				{
					// report Grid error back to the component
					base.SetError(ex.Message);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "Failed to query grid for Study data");
				}

				base.AddResultsToTable(results);

				foreach (NBIASearchResult result in base.SearchResults)
				{
					if (base.CancelRequested)
						break;

					NBIAQueryParameters nbiaQueryParameters = _queryParameters.Clone();
					nbiaQueryParameters.StudyInstanceUID = new QueryData(result.Study.StudyInstanceUid, QueryPredicate.EQUAL_TO);

					base.EnqueueNextCommand(new QueryForPatientInfoCommand(nbiaQueryParameters, result));
					base.EnqueueNextCommand(new QueryForClinicalTrialProtocolDataCommand(nbiaQueryParameters, result));
					base.EnqueueNextCommand(new QueryForTrialDataProvenanceCommand(nbiaQueryParameters, result));
					base.EnqueueNextCommand(new QueryForClinicalTrialSiteDataCommand(nbiaQueryParameters, result));
					base.EnqueueNextCommand(new QueryForSeriesDataCommand(nbiaQueryParameters, result));
				}

				base.OnCommandExecuted();
			}
		}

		private class QueryForPatientInfoCommand : SearchCommand
		{
			private readonly NBIAQueryParameters _queryParameters;
			private readonly NBIASearchResult _result; // search result to update with query results

			public QueryForPatientInfoCommand(NBIAQueryParameters queryParameters, NBIASearchResult result)
			{
				_queryParameters = queryParameters;
				_result = result;
			}

			public override void Execute()
			{
				if (!base.CancelRequested)
				{
					NBIAPatient patientService = new NBIAPatient();

					try
					{
						DataTable dtStudies = patientService.getPatientInfo(_queryParameters, SearchSettings.Default.NBIADataServiceUrl);
						if (dtStudies != null && dtStudies.Rows.Count > 0)
						{
							_result.Patient.PatientId = dtStudies.Rows[0]["patientId"].ToString();
							_result.Patient.PatientsName = dtStudies.Rows[0]["patientName"].ToString();
							_result.Patient.PatientsSex = dtStudies.Rows[0]["patientSex"].ToString();
							if (!string.IsNullOrEmpty(dtStudies.Rows[0]["patientBirthDate"].ToString()))
							{
								try
								{
									_result.Patient.PatientBirthDate = DateTime.Parse(dtStudies.Rows[0]["patientBirthDate"].ToString());
								}
								catch (Exception)
								{
								}
							}
							base.OnResultUpdated(_result);
						}
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Failed to query grid for Patient data");
					}
				}
				base.OnCommandExecuted();
			}
		}

		private class QueryForClinicalTrialProtocolDataCommand : SearchCommand
		{
			private readonly NBIAQueryParameters _queryParameters;
			private readonly NBIASearchResult _result; // search result to update with query results

			public QueryForClinicalTrialProtocolDataCommand(NBIAQueryParameters queryParameters, NBIASearchResult result)
			{
				_queryParameters = queryParameters;
				_result = result;
			}

			public override void Execute()
			{
				if (!base.CancelRequested)
				{
					NBIAClinicalTrialProtocol clinicalTrailProtocolService = new NBIAClinicalTrialProtocol();

					try
					{
						DataTable dtStudies = clinicalTrailProtocolService.getClinicalTrialProtocolInfo(_queryParameters, SearchSettings.Default.NBIADataServiceUrl);
						if (dtStudies != null && dtStudies.Rows.Count > 0)
						{
							_result.ClinicalTrialProtocol.ProtocolId = dtStudies.Rows[0]["protocolId"].ToString();
							_result.ClinicalTrialProtocol.ProtocolName = dtStudies.Rows[0]["protocolName"].ToString();

							base.OnResultUpdated(_result);
						}
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Failed to query grid for ClinicalTrialProtocol data");
					}
				}
				base.OnCommandExecuted();
			}
		}

		private class QueryForTrialDataProvenanceCommand : SearchCommand
		{
			private readonly NBIAQueryParameters _queryParameters;
			private readonly NBIASearchResult _result; // search result to update with query results

			public QueryForTrialDataProvenanceCommand(NBIAQueryParameters queryParameters, NBIASearchResult result)
			{
				_queryParameters = queryParameters;
				_result = result;
			}

			public override void Execute()
			{
				if (!base.CancelRequested)
				{
					NBIATrialDataProvenance trailDataProvenanceService = new NBIATrialDataProvenance();

					try
					{
						DataTable dtStudies = trailDataProvenanceService.getTrialDataProvenanceInfo(_queryParameters, SearchSettings.Default.NBIADataServiceUrl);
						if (dtStudies != null && dtStudies.Rows.Count > 0)
						{
							// FIX: Backend returns collection of project names, so instead of using just the first,
							// we will return the query parameter as it should exist in the results collection.
							// This needs to be fixed on the back end.
							//_result.TrialDataProvenance.Project = dtStudies.Rows[0]["project"].ToString();
							_result.TrialDataProvenance.Project = _queryParameters.ProjectName.SelectedValue;

							base.OnResultUpdated(_result);
						}
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Failed to query grid for TrialDataProvenance data");
					}
				}
				base.OnCommandExecuted();
			}
		}

		private class QueryForClinicalTrialSiteDataCommand : SearchCommand
		{
			private readonly NBIAQueryParameters _queryParameters;
			private readonly NBIASearchResult _result; // search result to update with query results

			public QueryForClinicalTrialSiteDataCommand(NBIAQueryParameters queryParameters, NBIASearchResult result)
			{
				_queryParameters = queryParameters;
				_result = result;
			}

			public override void Execute()
			{
				if (!base.CancelRequested)
				{
					NBIAClinicalTrialSite clinicalTrialSiteService = new NBIAClinicalTrialSite();

					try
					{
						DataTable dtStudies = clinicalTrialSiteService.getClinicalTrialSiteInfo(_queryParameters, SearchSettings.Default.NBIADataServiceUrl);
						if (dtStudies != null && dtStudies.Rows.Count > 0)
						{
							_result.ClinicalTrialSite.SiteId = dtStudies.Rows[0]["siteId"].ToString();
							_result.ClinicalTrialSite.SiteName = dtStudies.Rows[0]["siteName"].ToString();

							base.OnResultUpdated(_result);
						}
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Failed to query grid for ClinicalTrialSiteData data");
					}
				}
				base.OnCommandExecuted();
			}
		}

		private class QueryForSeriesDataCommand : SearchCommand
		{
			private readonly NBIAQueryParameters _queryParameters;
			private readonly NBIASearchResult _result; // search result to update with query results

			public QueryForSeriesDataCommand(NBIAQueryParameters queryParameters, NBIASearchResult result)
			{
				_queryParameters = queryParameters;
				_result = result;
			}

			public override void Execute()
			{
				if (!base.CancelRequested)
				{
					NBIASeries seriesService = new NBIASeries();

					try
					{
						DataTable dtStudies = seriesService.getSeriesInfo(_queryParameters, SearchSettings.Default.NBIADataServiceUrl);
						if (dtStudies != null)
						{
							Console.WriteLine("Project: " + dtStudies.Rows[0].ToString());
							Dictionary<string, string> modalities = new Dictionary<string, string>();
							foreach (DataRow row in dtStudies.Rows)
							{
								if (!string.IsNullOrEmpty(row["modality"].ToString().Trim()))
									modalities[row["modality"].ToString().Trim()] = string.Empty;
							}
							if (modalities.Count > 0)
							{
								_result.Series.Modality = StringUtilities.Combine(modalities.Keys, ",");

								base.OnResultUpdated(_result);
							}

							// TODO: If we need to display slice thickness, this is the place to query for it
						}
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Error, ex, "Failed to query grid for Series data");
					}
				}
				base.OnCommandExecuted();
			}
		}
	}
}