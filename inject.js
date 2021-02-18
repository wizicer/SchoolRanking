var colleges = { /*colleges*/ };
$('.top-message-menu.iboss-xiaoxitongzhi').after('<a href=""javascript: void(0)"" id=""markcol"">🈯</a>');
let refreshColTags = function () {
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
    setTimeout(function () {
        refreshColTags();
        loopRefreshColTags();
    }, 3000);
}
loopRefreshColTags();
