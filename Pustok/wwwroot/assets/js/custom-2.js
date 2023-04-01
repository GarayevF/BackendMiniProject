﻿$(document).ready(function () {

    let categoryIds = [];
    let authorIds = [];
    let min = -1;
    let max = -1;
    let sortby = -1;

    console.log(window.location.pathname)
    $(document).on('click', '.basketRemover, .addbasket, .subbasket, .deletewish, .addwish, .addcompare, .addresseditbtn', function (e) {
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
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'basket') {
                        fetch(url.replace("deletebasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.cart-page-main-block').html(data2)
                                refreshSlick();
                            })
                    }
                })
        }
        else if ($(this).hasClass('addbasket')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.cart-dropdown-block').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'basket') {
                        fetch(url.replace("addbasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.cart-page-main-block').html(data2)
                                refreshSlick();
                            })
                    }
                })

        }
        else if ($(this).hasClass('subbasket')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.cart-dropdown-block').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'basket') {
                        fetch(url.replace("removebasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.cart-page-main-block').html(data2)
                                refreshSlick();
                            })
                    }
                })

        }
        else if ($(this).hasClass('deletewish')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.wishlist-content').html(data)
                })
        }
        else if ($(this).hasClass('addwish')) {
            e.preventDefault();
            $(this).find('i').css('color', 'green');
            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return;
                })

        }
        else if ($(this).hasClass('addcompare')) {
            e.preventDefault();
            //$(this).find('i').css('color', 'green');
            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return;
                })

        }
        else if ($(this).hasClass('addresseditbtn')) {
            e.preventDefault();

            if (!$('.addressesContainer').hasClass('d-none')) $('.addressesContainer').addClass('d-none')
            if (!$('.addressForm').hasClass('d-none')) $('.addressesContainer').addClass('d-none')

            $('.editAdress').removeClass('d-none')

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.editAdress').html(data)
                })
        }
    })
        .on('keyup', '.ProductCountInp', function (e) {
            e.preventDefault();
            let inpVal = $(this).val()
            let code = e.keyCode || e.which
            if ($.isNumeric(inpVal) && inpVal % 1 === 0 && inpVal > 0 && parseInt(inpVal) != NaN && inpVal.indexOf('.') === -1 && code != 190) {
                fetch("/basket/BasketCartRefresh/" + $(this).attr("data-id") + "?count=" + inpVal)
                    .then(res => {
                        return res.text()
                    })
                    .then(data => {
                        $('.cart-dropdown-block').html(data)
                        if (window.location.pathname.split('/')[1].toLowerCase() === 'basket') {
                            fetch("/basket/mainbasket/" + $(this).attr("data-id"))
                                .then(res2 => {
                                    return res2.text()
                                })
                                .then(data2 => {
                                    $('.cart-page-main-block').html(data2)
                                    refreshSlick();
                                })
                        }

                    })
            }

        })
        
    $(document).on('click', '.addAddress', function (e) {
        e.preventDefault();

        $('.addressForm').removeClass('d-none');
        $('.addressesContainer').addClass('d-none');
    })

    $('.productModal').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => {
                return res.text()
            })
            .then(data => {
                $('.modal-content').html(data)
                $('.quick-view-image').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    dots: false,
                    fade: true,
                    asNavFor: '.quick-view-thumb',
                    speed: 400,
                });

                $('.quick-view-thumb').slick({
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    asNavFor: '.quick-view-image',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true,
                    speed: 400,
                });
            })
    })

})

function refreshSlick() {
    $('.sb-slick-slider').not('.slick-initialized').slick({
        autoplay: true,
        autoplaySpeed: 8000,
        slidesToShow: 2,
        arrows: false,
        responsive: [
            { "breakpoint": 992, "settings": { "slidesToShow": 2 } },
            { "breakpoint": 768, "settings": { "slidesToShow": 3 } },
            { "breakpoint": 575, "settings": { "slidesToShow": 2 } },
            { "breakpoint": 480, "settings": { "slidesToShow": 1 } },
            { "breakpoint": 320, "settings": { "slidesToShow": 1 } }
        ]
    });
}