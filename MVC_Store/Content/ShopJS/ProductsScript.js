$(function () {

    /* Select product from specified category */

    $("#SelectCategory").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/shop/Products?catId=" + url;
        }
        return false;
    });

    /*-----------------------------------------------------------*/

    /* Confirm page deletion */

    $("a.delete").click(function () {
        if (!confirm("Confirm product deletion")) return false;
    });

    /*-----------------------------------------------------------*/
});