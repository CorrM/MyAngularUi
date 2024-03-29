﻿using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Types;

namespace MAU.ReadyComponents;

public class MauTable : MauComponent
{
    #region [ Mau Variable ]

    [MauVariable("Content")]
    public MauDataTable Content { get; }

    #endregion

    public MauTable(string mauId) : base(mauId)
    {
        Content = new MauDataTable(this, nameof(Content));
    }
}