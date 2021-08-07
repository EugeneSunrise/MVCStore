$(function () {

    /*
     Confirm page deletion
     */

    $("a.delete").click(function () {
        if (!confirm("Confirm page deletion")) return false;
    });

    /////////////////////////////////////////////////////////////

    /*
     Sorting script
     */

    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            //console.log(ids);
            var url = "/Admin/Pages/ReorderPages";

            $.post(url, ids, function (data) {
            });
        }
    });
});