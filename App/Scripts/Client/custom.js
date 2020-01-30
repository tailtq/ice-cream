const swalWithBootstrapButtons = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-success',
        cancelButton: 'btn btn-danger'
    },
    buttonsStyling: false
});

function initAddToCart() {
    function renderItems(items) {
        items.forEach(function (item) {
            const $element = $(`.single-cart-block[data-id="${item.Id}"]`);
            if ($element.length > 0) {
                $element.find('.qty').html(`${item.Quantity} ×`);
            } else {
                $('.cart-dropdown-block .cart-items').append(`
                    <div class="single-cart-block" data-id="${item.Id}">
                        <div class="cart-product">
                            <a href="product-details.html" class="image">
                                <img src="${item.Image}" alt="${item.Name}">
                            </a>
                            <div class="content">
                                <h3 class="title">
                                    <a href="/Books/Detail/${item.Slug}-${item.Id}">
                                        ${item.Name}
                                    </a>
                                </h3>
                                <p class="price"><span class="qty">${item.Quantity} ×</span> $${parseFloat(item.Price - item.Price * item.Discount / 100).toFixed(2)}</p>
                            </div>
                        </div>
                    </div>
                `);
            }
        });
    }

    $('.book__add-to-cart').on('click', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        $.post(`/Cart/AddBook/${id}`, {
            Quantity: $('[name="Quantity"]').val() || 1
        }).then((res) => {
            if (res.Status) {
                const cart = res.Data;
                $('#cart-quantity').html(cart.Quantity);
                $('#cart-price').html(`$${parseFloat(cart.Total).toFixed(2)} <i class="fas fa-chevron-down"></i>`);
                renderItems(cart.ListItem);
            } else if (res.StatusCode === 400 && res.Message === 'NOT_ENOUGH_BOOK') {
                console.log(swalWithBootstrapButtons);
                swalWithBootstrapButtons.fire('Error', 'Sorry!<br>Quantity of this book is lower than your demand.', 'error');
            }
        });
    });
}

function initGetCart() {
    let cart;
    const elements = document.cookie.split(';');

    elements.forEach((element) => {
        const cartIndex = element.indexOf('cart=');
        if (element.indexOf('cart=') >= 0) {
            cart = JSON.parse(element.substr(cartIndex + 5));
        }
    });
    if (cart) {
        $('input[name="item.Quantity"]').on('change', function () {
            const quantity = parseInt($(this).val());
            const $tr = $(this).closest('tr');
            const id = $tr.data('id');

            if (quantity) {
                cart.ListItem.forEach((item) => {
                    if (id == item.Id) {
                        item.Quantity = quantity;
                        const total = calculateTotal(item.Price, item.Quantity, item.Discount);
                        $tr.find('.pro-subtotal').html(`$${getMoneyFormat(total)}`);
                        calculateCart();
                    }
                });
            }
        });

        $('.cart__item-remove').on('click', function (e) {
            e.preventDefault();
            const id = $(this).closest('tr').data('id');
            $(this).closest('tr').hide();

            cart.ListItem = cart.ListItem.filter(item => id != item.Id);
            calculateCart();
        });

        $('.cart__update').on('click', function () {
            $.post('/Cart', cart).then((res) => {
                if (res.Status) {
                    window.location.reload();
                } else if (res.StatusCode === 400 && res.Message === 'NOT_ENOUGH_BOOK') {
                    swalWithBootstrapButtons.fire('Error', 'Sorry!<br>Quantity of this book is lower than your demand.', 'error');
                }
            });
        });
    }

    function calculateTotal(price, quantity, discount) {
        return quantity * (price - price * discount / 100);
    }

    function calculateCart() {
        cart.Total = 0;
        cart.Quantity = 0;
        cart.ListItem.forEach((item) => {
            cart.Total += calculateTotal(item.Price, item.Quantity, item.Discount);
            cart.Quantity += item.Quantity;
        });
        $('#cartTotal').html(`$${getMoneyFormat(cart.Total)}`);
        // json + view
    }

    function getMoneyFormat(money) {
        return parseFloat(money).toFixed(2);
    }
}

function initCancelOrder() {
    $('.order__cancel').on('click', function (e) {
        e.preventDefault();
        Swal.fire({
            title: 'Enter the reason',
            input: 'text',
            showCancelButton: true,
            inputValidator: (value) => {
                if (!value) {
                    return 'You need to write something!'
                }
            }
        }).then((res) => {
            if (res.value) {
                const id = $(this).closest('tr').data('id');
                $.post(`/Customer/CancelOrder/${id}`, {
                    message: res.value,
                }).then((res) => {
                    if (res.Status) {
                        window.location.reload();
                    } else {
                        swalWithBootstrapButtons.fire('Error', '', 'error');
                    }
                });
            }
        });
    });
}

function initAddFavourite() {
    $('.addFavourite').click(function (e) {
        e.preventDefault();
        var itemType = $(this).data('item-type');
        var itemId = $(this).data('id');

        if (itemType) {
            const element = (itemType == 1) ? 'Books' : 'Flavors';

            $.post(`/${element}/Favourite/${itemId}`, {
                itemtype: itemType
            }).then(res => {
                if (res.Data == '1') {
                    $(this).addClass('favourite_added');
                } else if (res.Data == '0') {
                    $(this).removeClass('favourite_added');
                }
            })
            .catch(err => {
                console.log(err);
            });
        }
    })

}

$(document).ready(function () {
    initAddToCart();
    initGetCart();
    initCancelOrder();
    initAddFavourite();
});
