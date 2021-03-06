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
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace SearchComponent.View.WinForms
{
	public partial class AddEntityForm : Form
	{
		private readonly IList<ICodeSequenceItem> _allItems;

		public AddEntityForm(IList<ICodeSequenceItem> codeSequenceItems)
		{
			InitializeComponent();

			_allItems = new List<ICodeSequenceItem>(codeSequenceItems);
			_items.DataSource = codeSequenceItems;

			_items.DoubleClick += new EventHandler(OnItemDoubleClick);
			base.AcceptButton = _search;
			base.CancelButton = _cancel;
		}

		public IEnumerable<ICodeSequenceItem> SelectedItems
		{
			get
			{
				foreach (DataGridViewRow row in _items.SelectedRows)
				{
					yield return row.DataBoundItem as ICodeSequenceItem;
				}
			}
		}

		private void OnItemDoubleClick(object sender, EventArgs e)
		{
			Close();
		}

		private void _ok_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void _cancel_Click(object sender, EventArgs e)
		{
			_items.ClearSelection();
			this.Close();
		}

		private void _search_Click(object sender, EventArgs e)
		{
			_items.DataSource = CollectionUtils.Select(_allItems,
				delegate(ICodeSequenceItem item)
				{
					return (item.CodeMeaning ?? "").Contains(_searchText.Text) ||
						(item.CodeValue ?? "").Contains(_searchText.Text) ||
						(item.CodingSchemeDesignator ?? "").Contains(_searchText.Text);
				});
		}
	}
}
