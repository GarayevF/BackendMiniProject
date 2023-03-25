$(document).ready(function () {
    console.log('kjdfhkjdsfh')
    $(document).on('click', '.basketRemover, .addbasket', function (e) {
        console.log($(this))
        console.log($(this).hasClass('addbasket'))
        if ($(this).hasClass('basketRemover')) {
            e.preventDefault();

            let url = $(this).attr('href');
            console.log(url)
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.cart-dropdown-block').html(data)
                    console.log(url.replace("removebasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                    fetch(url.replace("removebasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                        .then(res2 => {
                            return res2.text()
                        })
                        .then(data2 => {
                            $('.cart-main-checkoutpage').html(data2)
                            console.log(url.replace("removebasket/" + url.split('/')[url.split('/').length - 1], 'MainBasketPrice'))
                            fetch(url.replace("removebasket/" + url.split('/')[url.split('/').length - 1], 'MainBasketPrice'))
                                .then(res3 => {
                                    return res3.text()
                                })
                                .then(data3 => {
                                    $('.checkout-price-div').html(data3)
                                })
                        })
                })
        } else if ($(this).hasClass('addbasket')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.cart-dropdown-block').html(data)
                })
        }
    })

})