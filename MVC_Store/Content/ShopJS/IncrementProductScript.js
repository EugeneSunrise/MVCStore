/* Increment product */
$(function () {

    $("a.incproduct").click(function (e) {
        e.preventDefault();

        var productId = $(this).data("id");
        var url = "/cart/IncrementProduct";

        $.getJSON(url,
            { productId: productId },
            function (data) {
                $("td.qty" + productId).html(data.qty);

                var price = data.qty * data.price;
                var priceHtml = "€" + price.toFixed(2);

                $("td.total" + productId).html(priceHtml);

                var gt = parseFloat($("td.grandtotal span").text());
                var grandtotal = (gt + data.price).toFixed(2);

                $("td.grandtotal span").text(grandtotal);
            }).done(function (data) {
                var url2 = "/cart/PaypalPartial";

                $.get(url2,
                    {},
                    function (data) {
                        $("div.paypaldiv").html(data);
                    });
            });
    });
    /*-----------------------------------------------------------*/

    /* Decriment product */
    $(function () {

        $("a.decproduct").click(function (e) {
            e.preventDefault();

            var $this = $(this);
            var productId = $(this).data("id");
            var url = "/cart/DecrementProduct";

            $.getJSON(url,
                { productId: productId },
                function (data) {

                    if (data.qty == 0) {
                        $this.parent().fadeOut("fast",
                            function () {
                                location.reload();
                            });
                    } else {
                        $("td.qty" + productId).html(data.qty);

                        var price = data.qty * data.price;
                        var priceHtml = "€" + price.toFixed(2);

                        $("td.total" + productId).html(priceHtml);

                        var gt = parseFloat($("td.grandtotal span").text());
                        var grandtotal = (gt - data.price).toFixed(2);

                        $("td.grandtotal span").text(grandtotal);
                    }
                }).done(function (data) {

                    var url2 = "/cart/PaypalPartial";

                    $.get(url2,
                        {},
                        function (data) {
                            $("div.paypaldiv").html(data);
                        });
                });
        });
    });
    /*-----------------------------------------------------------*/

    /* Remove product */
    $(function () {

        $("a.removeproduct").click(function (e) {
            e.preventDefault();

            var $this = $(this);
            var productId = $(this).data("id");
            var url = "/cart/RemoveProduct";

            $.get(url,
                { productId: productId },
                function (data) {
                    location.reload();
                });
        });
    });

    /* Place order */
    $(function () {

        $("a.placeorder").click(function (e) {
            e.preventDefault();

            var $this = $(this);
            var url = "/cart/PlaceOrder";

            $(".ajaxbg").show();

            $.post(url,
                {},
                function (data) {
                    $(".ajaxbg span").text("Thank you. You will now be redirected to paypal.");
                    setTimeout(function () {
                        $('form input[name = "submit"]').click();
                    }, 2000);
                });
        });
    });

});
        /*-----------------------------------------------------------*/