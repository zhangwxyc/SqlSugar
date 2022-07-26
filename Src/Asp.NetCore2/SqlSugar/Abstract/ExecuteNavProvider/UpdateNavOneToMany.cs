﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class UpdateNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        public NavigateType? _NavigateType { get; set; }
        private void UpdateOneToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            List<TChild> children = new List<TChild>();
            var parentEntity = _ParentEntity;
            var parentList = _ParentList;
            var parentNavigateProperty = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == name);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            var thisPkColumn = GetPkColumnByNav(thisEntity, nav);
            var thisFkColumn = GetFKColumnByNav(thisEntity, nav);
            EntityColumnInfo parentPkColumn = GetParentPkColumn();
            EntityColumnInfo parentNavColumn = GetParentPkNavColumn(nav);
            if (parentNavColumn != null)
            {
                parentPkColumn = parentNavColumn;
            }
            var ids = new List<object>();
            foreach (var item in parentList)
            {
                var parentValue = parentPkColumn.PropertyInfo.GetValue(item);
                var childs = parentNavigateProperty.PropertyInfo.GetValue(item) as List<TChild>;
                if (childs != null)
                {
                    foreach (var child in childs)
                    {
                        thisFkColumn.PropertyInfo.SetValue(child, parentValue, null);
                    }
                    children.AddRange(childs);
                }
                ids.Add(parentValue);
            }
            this._Context.Deleteable<object>()
                .AS(thisEntity.DbTableName)
                .In(thisFkColumn.DbColumnName, ids.Distinct().ToList()).ExecuteCommand();
            _NavigateType = NavigateType.OneToMany;
            InsertDatas(children, thisPkColumn);
            _NavigateType = null;
            SetNewParent<TChild>(thisEntity, thisPkColumn);
        }

        private EntityColumnInfo GetParentPkColumn()
        {
            EntityColumnInfo parentPkColumn = _ParentPkColumn;
            if (_ParentPkColumn == null)
            {
                parentPkColumn = _ParentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            }
            return parentPkColumn;
        }
        private EntityColumnInfo GetParentPkNavColumn(EntityColumnInfo nav)
        {
            EntityColumnInfo result = null;
            if (nav.Navigat.Name2.HasValue())
            {
                result = _ParentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name2);
            }
            return result;
        }

        private void SetNewParent<TChild>(EntityInfo entityInfo, EntityColumnInfo entityColumnInfo) where TChild : class, new()
        {
            this._ParentEntity = entityInfo;
            this._ParentPkColumn = entityColumnInfo;
        }
    }
}
