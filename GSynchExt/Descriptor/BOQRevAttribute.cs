using System;
using System.Collections;
using PX.Data;
using PX.Objects.CS;


namespace PX.Objects.PM
{
    /// <summary>
    /// BOQ Revision PX Selector Attribute
    /// </summary>

    public class BOQRev
    {
        public class IDAttribute : PXAggregateAttribute, IPXFieldDefaultingSubscriber
        {
            private PXDefaultAttribute _DefaultAttribute;
            private Selector _Selector;
            /// <summary>
            /// Selector Description Field
            /// </summary>
            public Type DescriptionField
            {
                get
                {
                    return _Selector?.DescriptionField;
                }
                set
                {
                    _Selector.DescriptionField = value;
                }
            }

            public IDAttribute(Type defaultType, Type keyField, Type revisionField, params Type[] fieldList)
            {
                _Selector = new Selector(keyField, revisionField, fieldList);
                _Attributes.Add(_Selector);
                _DefaultAttribute = new PXDefaultAttribute(defaultType);
            }

            public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
            {
                _DefaultAttribute.FieldDefaulting(sender, e);
                e.Cancel = false;
            }

            #region "Sub-Attributes"

            public class Selector : PXCustomSelectorAttribute
            {
                private Type _KeyField;
                private Type _RevisionField;

                public Selector(Type keyField, Type revisionField, params Type[] fieldList) : base(revisionField, fieldList)
                {
                    _KeyField = keyField;
                    _RevisionField = revisionField;
                    ValidateValue = false;
                }
                public IEnumerable GetRecords()
                {
                    var cache = _Graph.Caches[_BqlTable];
                    var keyFieldValue = cache.GetValue(cache.Current, _KeyField.Name);
                    var select = BqlCommand.Compose(
                                    typeof(Select<,,>),
                                        _BqlTable,
                                    typeof(Where<,>),
                                        _KeyField,
                                    typeof(Equal<>),
                                        typeof(Required<>),
                                            _KeyField,
                                    typeof(OrderBy<>),
                                        typeof(Desc<>),
                                        _RevisionField
                                    );

                    var cmd = BqlCommand.CreateInstance(select);
                    PXView view = new PXView(_Graph, true, cmd);

                    foreach (var ret in view.SelectMulti(keyFieldValue))
                    {
                        yield return ret;
                    }
                }
            }
            public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
            {
                if (e.Row == null)
                    return;

                int? costCode = (int?)sender.GetValue(e.Row, _FieldName);
                if (costCode == null)
                    return;

                if (DescriptionField != null)
                {
                    string desc = (string)sender.GetValue(e.Row, DescriptionField.Name);
                    if (desc == null)
                    {
                        return;
                    }
                }

                #endregion
            }
        }

        public class KeyAttribute : PXAggregateAttribute
        {
            private KeyAttribute(Type numberingSeq)
            {
                _Attributes.Add(new Numbering(numberingSeq, typeof(AccessInfo.businessDate)));
            }

            public KeyAttribute(Type numberingSeq, Type keyField, Type revisionField, params Type[] fieldList) : this(numberingSeq)
            {
                _Attributes.Add(new Selector(keyField, revisionField, fieldList));
            }

            public KeyAttribute(Type numberingSeq, Type keyField, Type revisionField, Type where, params Type[] fieldList) : this(numberingSeq)
            {
                if (where == null)
                {
                    throw new ArgumentNullException(nameof(where));
                }

                _Attributes.Add(new Selector(keyField, revisionField, where, fieldList));
            }

            public void InsertRevision()
            {
                GetAttribute<Numbering>().SetAutoNumbering(false);
            }

            public string GetNewSymbol()
            {
                return GetAttribute<Numbering>().GetNewSymbol();
            }

            public bool IsAutoNumber()
            {
                return GetAttribute<Numbering>().IsAutoNumber();
            }

            public static void InsertRevision<TField>(PXCache cache)
                where TField : IBqlField
            {
                foreach (var keyAttribute in cache.GetAttributes<TField>())
                {
                    if (keyAttribute is KeyAttribute)
                    {
                        var autoNumberAttribute = (KeyAttribute)keyAttribute;
                        autoNumberAttribute.InsertRevision();
                    }
                }
            }

            public static string GetNewSymbol<TField>(PXCache cache)
                where TField : IBqlField
            {
                foreach (var keyAttribute in cache.GetAttributes<TField>())
                {
                    if (keyAttribute is KeyAttribute)
                    {
                        var autoNumberAttribute = (KeyAttribute)keyAttribute;
                        return autoNumberAttribute.GetNewSymbol();
                    }
                }
                return string.Empty;
            }

            public static bool GetIsAutoNumber<TField>(PXCache cache)
                where TField : IBqlField
            {
                foreach (var keyAttribute in cache.GetAttributes<TField>())
                {
                    if (keyAttribute is KeyAttribute)
                    {
                        var autoNumberAttribute = (KeyAttribute)keyAttribute;
                        return autoNumberAttribute.IsAutoNumber();
                    }
                }
                return false;
            }

            #region "Key Attributes"

            private class Selector : PXCustomSelectorAttribute
            {
                private Type _RevisionField;
                private Type _Where;
                public Selector(Type keyField, Type revisionField, params Type[] fieldList)
                    : this(keyField, revisionField, typeof(Where<True, Equal<True>>), fieldList)
                {
                }

                public Selector(Type keyField, Type revisionField, Type where, params Type[] fieldList)
                    : base(keyField, fieldList)
                {
                    _RevisionField = revisionField;
                    ValidateValue = false;
                    _Where = where;
                }
                public IEnumerable GetRecords()
                {
                    var cache = _Graph.Caches[_BqlTable];

                    var select = BqlCommand.Compose(
                                    typeof(Select<,,>),
                                        _BqlTable, _Where,
                                    typeof(OrderBy<>),
                                        typeof(Asc<,>),
                                        this.Field,
                                        typeof(Desc<>),
                                            _RevisionField
                                    );

                    var cmd = BqlCommand.CreateInstance(select);
                    PXView view = new PXView(_Graph, true, cmd);

                    var prevKey = string.Empty;
                    foreach (var ret in view.SelectMulti())
                    {
                        var curKey = cache.GetValue(ret, _FieldOrdinal) as string;
                        if (curKey != prevKey || _Graph.IsExport)
                            yield return ret;
                        prevKey = curKey;
                    }
                }
            }

            private class Numbering : AutoNumberAttribute
            {
                private bool _IsAutoNumbering = true;
                public Numbering(Type setupField, Type dateField) : base(setupField, dateField) { }

                public void SetAutoNumbering(bool isActivated)
                {
                    _IsAutoNumbering = isActivated;
                }

                public string GetNewSymbol()
                {
                    return NewSymbol;
                }

                public bool IsAutoNumber()
                {
                    return UserNumbering != true;
                }

                protected override void Parameter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
                {
                    if (_IsAutoNumbering)
                    {
                        base.Parameter_RowSelected(sender, e);
                    }
                }

                public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
                {
                    var newKey = sender.GetValue(e.Row, _FieldOrdinal) as string;
                    if (this.NewSymbol == null || newKey?.Contains(this.NewSymbol) == true)
                        base.RowPersisting(sender, e);
                }

                public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
                {
                    //not sure what this code is doing, removing it for now to prevent bom fields from clearing out on error

                    //var newKey = sender.GetValue(e.Row, _FieldOrdinal) as string;
                    //if (this.NewSymbol == null || newKey?.Contains(this.NewSymbol) == true)
                    base.RowPersisted(sender, e);
                    //else
                    //SetAutoNumbering(true);
                }

            }

            #endregion

        }
        public class Key2Attribute : PXAggregateAttribute
        {
            private Key2Attribute(Type numberingSeq)
            {
                _Attributes.Add(new Numbering(numberingSeq, typeof(AccessInfo.businessDate)));
            }

            public Key2Attribute(Type numberingSeq, Type keyField, params Type[] fieldList) : this(numberingSeq)
            {
                _Attributes.Add(new Selector(keyField, fieldList));
            }

            public Key2Attribute(Type numberingSeq, Type keyField, Type where, params Type[] fieldList) : this(numberingSeq)
            {
                if (where == null)
                {
                    throw new ArgumentNullException(nameof(where));
                }

                _Attributes.Add(new Selector(keyField, where, fieldList));
            }

            public void InsertRevision()
            {
                GetAttribute<Numbering>().SetAutoNumbering(false);
            }

            public string GetNewSymbol()
            {
                return GetAttribute<Numbering>().GetNewSymbol();
            }

            public bool IsAutoNumber()
            {
                return GetAttribute<Numbering>().IsAutoNumber();
            }

            public static void InsertRevision<TField>(PXCache cache)
                where TField : IBqlField
            {
                foreach (var keyAttribute in cache.GetAttributes<TField>())
                {
                    if (keyAttribute is Key2Attribute)
                    {
                        var autoNumberAttribute = (Key2Attribute)keyAttribute;
                        autoNumberAttribute.InsertRevision();
                    }
                }
            }

            public static string GetNewSymbol<TField>(PXCache cache)
                where TField : IBqlField
            {
                foreach (var keyAttribute in cache.GetAttributes<TField>())
                {
                    if (keyAttribute is Key2Attribute)
                    {
                        var autoNumberAttribute = (Key2Attribute)keyAttribute;
                        return autoNumberAttribute.GetNewSymbol();
                    }
                }
                return string.Empty;
            }

            public static bool GetIsAutoNumber<TField>(PXCache cache)
                where TField : IBqlField
            {
                foreach (var keyAttribute in cache.GetAttributes<TField>())
                {
                    if (keyAttribute is Key2Attribute)
                    {
                        var autoNumberAttribute = (Key2Attribute)keyAttribute;
                        return autoNumberAttribute.IsAutoNumber();
                    }
                }
                return false;
            }

            #region "Key Attributes"

            private class Selector : PXCustomSelectorAttribute
            {
                private Type _Where;
                public Selector(Type keyField, params Type[] fieldList)
                    : this(keyField, typeof(Where<True, Equal<True>>), fieldList)
                {
                }

                public Selector(Type keyField, Type where, params Type[] fieldList)
                    : base(keyField, fieldList)
                {
                    //_RevisionField = revisionField;
                    ValidateValue = false;
                    _Where = where;
                }
                public IEnumerable GetRecords()
                {
                    var cache = _Graph.Caches[_BqlTable];

                    var select = BqlCommand.Compose(
                                    typeof(Select<,,>),
                                        _BqlTable, _Where,
                                    typeof(OrderBy<>),
                                        typeof(Asc<>),
                                        this.Field
                                    );

                    var cmd = BqlCommand.CreateInstance(select);
                    PXView view = new PXView(_Graph, true, cmd);

                    var prevKey = string.Empty;
                    foreach (var ret in view.SelectMulti())
                    {
                        var curKey = cache.GetValue(ret, _FieldOrdinal) as string;
                        if (curKey != prevKey || _Graph.IsExport)
                            yield return ret;
                        prevKey = curKey;
                    }
                }
            }

            private class Numbering : AutoNumberAttribute
            {
                private bool _IsAutoNumbering = true;
                public Numbering(Type setupField, Type dateField) : base(setupField, dateField) { }

                public void SetAutoNumbering(bool isActivated)
                {
                    _IsAutoNumbering = isActivated;
                }

                public string GetNewSymbol()
                {
                    return NewSymbol;
                }

                public bool IsAutoNumber()
                {
                    return UserNumbering != true;
                }

                protected override void Parameter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
                {
                    if (_IsAutoNumbering)
                    {
                        base.Parameter_RowSelected(sender, e);
                    }
                }

                public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
                {
                    var newKey = sender.GetValue(e.Row, _FieldOrdinal) as string;
                    if (this.NewSymbol == null || newKey?.Contains(this.NewSymbol) == true)
                        base.RowPersisting(sender, e);
                }

                public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
                {
                    //not sure what this code is doing, removing it for now to prevent bom fields from clearing out on error

                    //var newKey = sender.GetValue(e.Row, _FieldOrdinal) as string;
                    //if (this.NewSymbol == null || newKey?.Contains(this.NewSymbol) == true)
                    base.RowPersisted(sender, e);
                    //else
                    //SetAutoNumbering(true);
                }

            }

            #endregion

        }
    }
}
