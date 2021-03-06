﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ator.DbEntity.Sys
{
    [Table("Sys_Dictionary")]
    [SqlSugar.SugarTable("Sys_Dictionary")]
    public class SysDictionary : EntityBase
    {
        [Key]
        [StringLength(32)]
        [SugarColumn(IsPrimaryKey = true)]
        public string SysDictionaryId { get; set; } 

        [Display(Name = "字典名称")]
        [StringLength(32)]
        public string SysDictionaryName { get; set; }

        [Display(Name = "字典分组")]
        [StringLength(32)]
        public string SysDictionaryGroup { get; set; }
    }
}
