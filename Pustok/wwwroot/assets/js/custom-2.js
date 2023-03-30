$(document).ready(function () {

    let categoryIds = [];
    let authorIds = [];
    let min = -1;
    let max = -1;
    let sortby = -1;

    console.log(window.location.pathname)
    $(document).on('click', '.basketRemover, .addbasket, .subbasket, .deletewish, .addwish, .addcompare, .paginated-btn, .categorybtn, .authorbtn', function (e) {
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
        else if ($(this).hasClass('paginated-btn')) {
            e.preventDefault();

            let url = $(this).attr('href');


            let path = [];

            if (categoryIds.length > 0) {
                path.push(`categoriesText=${categoryIds.join(",")}`);
            }

            if (authorIds.length > 0) {
                path.push(`authorsText=${authorIds.join(",")}`);
            }

            if (min > -1) {
                path.push(`min=${min}`)
            }

            if (max > -1) {
                path.push(`max=${max}`)
            }

            sortby = $('.nice-select.sort-select option:selected').val();

            if (sortby > -1) {
                path.push(`sortby=${sortby}`)
            }

            fetch(`${url}&${path.join('&')}`)
                .then(res => {
                    return res.text();
                })
                .then(data => {
                    $('.shopProductList').html(data)
                })

        }
        else if ($(this).hasClass('categorybtn')) {
            e.preventDefault();

            let url = $(this).attr('href');
            let ctId = url.split('/')[url.split('/').length - 1];

            if (categoryIds.indexOf(ctId) > -1) {
                categoryIds.splice(categoryIds.indexOf(ctId), 1);
                $(this).css('color', '#555555');
                $(this).css('font-weight', '400');
            } else {
                categoryIds.push(ctId);
                $(this).css('color', 'green');
                $(this).css('font-weight', 'bold');
            }

            let path = [];

            if (categoryIds.length > 0) {
                path.push(`categoriesText=${categoryIds.join(",")}`);
            }

            if (authorIds.length > 0) {
                path.push(`authorsText=${authorIds.join(",")}`);
            }

            if (min > -1) {
                path.push(`min=${min}`)
            }

            if (max > -1) {
                path.push(`max=${max}`)
            }

            sortby = $('.nice-select.sort-select option:selected').val();

            if (sortby > -1) {
                path.push(`sortby=${sortby}`)
            }

            fetch(`${url}?${path.join('&')}`)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.shopProductList').html(data)
                })
        }
        else if ($(this).hasClass('authorbtn')) {
            e.preventDefault();

            let url = $(this).attr('href');
            let authId = url.split('/')[url.split('/').length - 1];

            if (authorIds.indexOf(authId) > -1) {
                authorIds.splice(authorIds.indexOf(authId), 1);
                $(this).css('color', '#555555');
                $(this).css('font-weight', '400');
            } else {
                authorIds.push(authId);
                $(this).css('color', 'green');
                $(this).css('font-weight', 'bold');
            }

            let path = [];

            if (categoryIds.length > 0) {
                path.push(`categoriesText=${categoryIds.join(",")}`);
            }

            if (authorIds.length > 0) {
                path.push(`authorsText=${authorIds.join(",")}`);
            }

            if (min > -1) {
                path.push(`min=${min}`)
            }

            if (max > -1) {
                path.push(`max=${max}`)
            }

            sortby = $('.nice-select.sort-select option:selected').val();

            if (sortby > -1) {
                path.push(`sortby=${sortby}`)
            }

            fetch(`${url}?${path.join('&')}`)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.shopProductList').html(data)
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
        .on('change', '.nice-select', function (e) {
            e.preventDefault();

            let path = [];

            if (categoryIds.length > 0) {
                path.push(`categoriesText=${categoryIds.join(",")}`);
            }

            if (authorIds.length > 0) {
                path.push(`authorsText=${authorIds.join(",")}`);
            }

            if (min > -1) {
                //if min is numric
                path.push(`min=${min}`)
            }

            if (max > -1) {
                path.push(`max=${max}`)
            }

            sortby = $('.nice-select.sort-select option:selected').val();
            

            if (sortby > -1) {
                path.push(`sortby=${sortby}`)
            }
            alert(`product/filter?${path.join('&')}`)
            fetch(`product/filter?${path.join('&')}`)
                .then(res => {
                    return res.text();
                })
                .then(data => {
                    $('.shopProductList').html(data)
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