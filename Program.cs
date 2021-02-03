﻿namespace SchoolRanking
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using CsvHelper.Configuration.Attributes;
    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {
            var cs = GetRecords<CollegeCsv>(@"..\..\..\college.csv");
            var rs = GetRecords<Ranking>(@"..\..\..\ranking.csv");
            var colleges = cs.Select(_ => _.Name)
                .Concat(rs.Select(_ => _.Name))
                .Distinct()
                .Select(name => new
                {
                    name,
                    c = cs.FirstOrDefault(_ => _.Name == name),
                    r = rs.FirstOrDefault(_ => _.Name == name),
                })
                .Select(o => new College(
                    o.name,
                    o.c?.Index ?? -1,
                    o.r?.Rank,
                    o.c?.Location ?? o.r.Location,
                    o.c?.Is211 == "211",
                    o.c?.Is985 == "985",
                    o.c?.Type,
                    o.c?.Category,
                    o.c?.Comment,
                    o.c?.Level))
                .ToArray();
            var jsonColleges = colleges
                .Select(_ => new CollegeJson(_.Name,
                    _.Is211 ? "211" : null,
                    _.Is985 ? "985" : null,
                    _.Index == -1 ? null : $"IDX{_.Index}",
                    //_.Location,
                    _.Ranking == null ? null : $"RNK{_.Ranking}"
                    ))
                .Select(_ => _ with { Tags = _.Tags.Where(t => t != null).ToArray() })
                .ToDictionary(_ => _.Name, _ => _.Tags);

            var s = $"var colleges = JSON.parse('{JsonConvert.SerializeObject(jsonColleges)}');" + @"
$('.top-message-menu.iboss-xiaoxitongzhi').after('<a href=""javascript: void(0)"" id=""markcol"">🈯</a>');
let refreshColTags = function(){
    let nameElm = $('.chat-geek-card .card-brief-item').first();
    let colname = nameElm.text().substring(5);
    let col = colleges[colname];
    if (nameElm.parent().find('span.col-badge').length == 0) {
        if (!col) {
            col = ['🈚'];
        }
        for (var i = 0; i < col.length; i++) {
            nameElm.after(`<span class=""col-badge"">▫️${col[i]}</span>`)
            $('.geek-info.card-brief-item').after(`<span class=""col-badge"">▫️${col[i]}</span>`);
        }
    }
}
$('#markcol').click(() => refreshColTags());
function loopRefreshColTags() {
    setTimeout(function() {
        refreshColTags();
        loopRefreshColTags();
    }, 3000);
}
loopRefreshColTags();
";
        }

        private static T[] GetRecords<T>(string input)
        {
            using var reader = new StreamReader(input);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            return csv.GetRecords<T>()
                .ToArray();
        }

    }

    [DebuggerDisplay("{Name}")]
    public record CollegeJson(
        string Name,
        params string[] Tags
    );

    [DebuggerDisplay("{Name}")]
    public record College(
        string Name,
        int Index,
        string Ranking,
        string Location,
        bool Is211,
        bool Is985,
        string Type,
        string Category,
        string Competent,
        string Level
    );

    [DebuggerDisplay("{Name}")]
    public record CollegeCsv(
        [Name("序号")] int Index,
        [Name("学校名称")] string Name,
        [Name("省")] string Province,
        [Name("地区")] string Location,
        [Name("市")] string City,
        [Name("211")] string Is211,
        [Name("985")] string Is985,
        [Name("办学类型")] string Type,
        [Name("学校分类")] string Category,
        [Name("上级部门")] string Competent,
        [Name("学校类型")] string Level,
        [Name("备注")] string Comment
    );

    [DebuggerDisplay("{Name}")]
    public record Ranking(
        [Name("排名")] string Rank,
        [Name("学校名称")] string Name,
        [Name("省市")] string Location,
        [Name("学校类型")] string Category,
        [Name("总得分")] string Points
    );


}
