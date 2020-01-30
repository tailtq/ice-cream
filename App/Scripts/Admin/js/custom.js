const swalWithBootstrapButtons = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-success',
        cancelButton: 'btn btn-danger'
    },
    buttonsStyling: false
});

function initShowElement() {
    $('.element__show').on('click', function () {
        let content = ``;
        const data = $(this).data('element');
        console.log(data);
        Object.keys(data).forEach((key) => {
            content += `
                <tr>
                    <th>${key}</th>
                    <td>${data[key]}</td>
                </tr>`;
        });
        $('#showElement').find('table tbody').html(content);
    });
}

function initDeleteElement() {
    $('.element__delete').on('click', function () {
        const id = $(this).closest('tr').data('id');
        const module = $(this).closest('table').data('module');
        swalWithBootstrapButtons.fire({
            title: 'Are you sure?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            reverseButtons: true
        }).then((result) => {
            if (!result.value) return;
            $.post(`/Admin/${module}/Delete/${id}`).then((res) => {
                if (res.Status) {
                    swalWithBootstrapButtons.fire('Succeeded!', '', 'success');
                    $(this).closest('tr').fadeOut()
                } else {
                    swalWithBootstrapButtons.fire('Error', '', 'error');
                }
            });
        });
        
    });
}

function initBlockElement() {
    $('.element__block').on('click', function () {
        const self = this;
        const id = $(this).closest('tr').data('id');
        const module = $(this).closest('table').data('module');

        swalWithBootstrapButtons.fire({
            title: 'Are you sure?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            reverseButtons: true
        }).then((result) => {
            if (!result.value) return;
            $.post(`/Admin/${module}/Block/${id}`).then((res) => {
                if (res.Status) {
                    swalWithBootstrapButtons.fire('Succeeded!', '', 'success');
                    if (!res.Data) {
                        $(self).removeClass('btn-success').addClass('btn-danger');
                        $(self).html('<i class="fas fa-trash"></i>');
                    } else {
                        $(self).addClass('btn-success').removeClass('btn-danger');
                        $(self).html('<i class="fas fa-trash-restore-alt"></i>');
                    }
                } else {
                    swalWithBootstrapButtons.fire('Error', '', 'error');
                }
            });
        });
    });
}

function initReadElement() {
    $('.element__read').on('click', function () {
        const id = $(this).closest('tr').data('id');
        const module = $(this).closest('table').data('module');
        $.post(`/Admin/${module}/Read/${id}`).then(() => $(this).fadeOut());
    });
}

function displayMessage(method) {
    $('.order__message')[method]();
    if (method == 'show' || method == 'fadeIn') {
        $('[name="Message"]').attr('required', true);
    } else {
        $('[name="Message"]').attr('required', false);
    }
}

function initViewOrder() {
    $('.element__show-order').on('click', function () {
        const element = $(this).data('element');
        let total = 0;
        let elementDetail = '';
        $('[name="Id"]').val(element.Id);
        $('[name="Status"]').val(element.Status);
        $('[name="Message"]').val(element.Message);

        if (element.Status != 5) {
            $('[name="Status"] > option[value="5"]').attr('disabled', true);
        } else {
            $('[name="Status"] > option[value="5"]').attr('disabled', false);
        }
        if (element.Status == 3 || element.Status == 4) {
            $('[name="Status"]').attr('disabled', true);
            $('[name="Message"]').attr('disabled', true);
            $('.order__change-status button[type="submit"]').hide();
        } else {
            if (element.Status == 5) {
                $('[name="Message"]').attr('disabled', true);
            } else {
                $('[name="Message"]').attr('disabled', false);
            }

            $('[name="Status"]').attr('disabled', false);
            $('.order__change-status button[type="submit"]').show();
        }
        if (element.Status == 4 || element.Status == 5) {
            displayMessage('show');
        } else {
            displayMessage('hide');
        }

        // Render HTML
        element.OrderDetails.forEach((detail, index) => {
            total += (detail.Price - detail.Price * detail.Discount / 100) * detail.Quantity;
            elementDetail += `<tr>
                <td>${index + 1}</td>
                <td>${detail.BookName}</td>
                <td class="text-center">${detail.Quantity}</td>
                <td class="text-center">$${detail.Price}</td>
                <td class="text-center">${detail.Discount}%</td>
            </tr>`
        });
        $('.order-detail').html(`
            <table class="table table-bordered">
                <tbody>
                    <tr>
                        <th>Code</th>
                        <td>${element.Code}</td>
                    </tr>
                    <tr>
                        <th>Customer</th>
                        <td>${element.CustomerName}</td>
                    </tr>
                    <tr>
                        <th>Ordered At:</th>
                        <td>${element.CreatedAt}</td>
                    </tr>
                </tbody>
            </table>
            <table class="table table-bordered">
                <thead>
                    <tr role="row">
                        <th class="text-center">#</th>
                        <th>Book</th>
                        <th class="text-center">Quantity</th>
                        <th class="text-center">Unit Price</th>
                        <th class="text-center">Discount</th>
                    </tr>
                </thead>
                <tbody>
                    ${elementDetail}
                    <tr>
                        <td colspan="3" class="text-right">Sub Total:</td>
                        <td colspan="2" class="text-center">$${getMoneyFormat(total)}</b></td>
                    </tr>
                    <tr>
                        <td colspan="3" class="text-right">Shipping Fee:</td>
                        <td colspan="2" class="text-center">$${getMoneyFormat(element.ShippingFee)}</b></td>
                    </tr>
                    <tr>
                        <td colspan="3" class="text-right">Tax:</td>
                        <td colspan="2" class="text-center">$${getMoneyFormat(element.Tax)}</td>
                    </tr>
                    <tr>
                        <td colspan="3" class="text-right"><b>Total:</b></td>
                        <td colspan="2" class="text-center"><b>$${getMoneyFormat(total + element.ShippingFee + element.Tax)}</b></td>
                    </tr>
                </tbody>
            </table>
            
        `)
    });
}

function initChangeOrderStatus() {
    $('[name="Status"]').on('change', function () {
        const value = $(this).val();

        if (value == 4 || value == 5) {
            displayMessage('fadeIn');
        } else {
            displayMessage('fadeOut');
        }
    });

    $('.order__change-status').on('submit', function (e) {
        e.preventDefault();
        const id = $('[name="Id"]').val();
        const status = $('[name="Status"]').val();
        const message = $('[name="Message"]').val();

        swalWithBootstrapButtons.fire({
            title: 'Are you sure?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            reverseButtons: true
        }).then((result) => {
            if (!result.value) return;
            $.post(`/Admin/Orders/UpdateStatus/${id}`, {
                Status: status,
                Message: message,
            }).then((res) => {
                if (res.Status) {
                    swalWithBootstrapButtons.fire('Succeeded!', '', 'success');

                    const $element = $(`[data-id="${res.Data.Id}"]`);
                    const element = $element.find('.element__show-order').data('element');
                    element.Status = res.Data.Status;
                    $element.find('.element__show-order').attr('data-element', JSON.stringify(element));
                    $element.find('.staffName').html(res.Data.StaffName);

                    switch (element.Status) {
                        case 2:
                            $element.find('.status').html('Delivering');
                            break;
                        case 3:
                            $element.find('.status').html('Received');
                            break;
                        case 4:
                            $element.find('.status').html('Canceled');
                            break;
                    }
                    $('#showElement').modal('hide');
                } else {
                    swalWithBootstrapButtons.fire('Error', '', 'error');
                }
            });
        })
    });
}

function initAddInputDetail() {
    const books = $('.book-data').data('books');
    let index = $('.input__details').children().length;
    $('.input__add-input-detail').on('click', function () {
        let options = '';
        books.forEach((book) => {
            options += `<option value="${book.Id}">${book.Sku}</option>`;
        });

        $('.input__details').append(`
            <div class="row input__detail mb-4">
                <div class="col-md-6">
                    <select class="form-control" name="InputDetails[${index}][BookId]">
                        ${options}
                    </select>
                </div>
                <div class="col-md-3">
                    <input type="number" name="InputDetails[${index}][Price]" value="" class="form-control" placeholder="Price" required/>
                </div>
                <div class="col-md-3">
                    <input type="number" name="InputDetails[${index}][Quantity]" value="" class="form-control" placeholder="Quantity" required />
                </div>
            </div>
        `);
        index++;
    });
}

function initApproveElement() {
    $('.element__approve').on('click', function () {
        const self = this;
        const id = $(this).closest('tr').data('id');
        const module = $(this).closest('table').data('module');

        Swal.fire({
            title: 'Enter the prize (USD)',
            input: 'number',
            showCancelButton: true,
            inputValidator: (value) => {
                if (!value) {
                    return 'You need to write something!'
                } else if (isNaN(value) || value <= 0) {
                    return 'Invalid prize';
                }
            }
        }).then((res) => {
            if (res.value) {
                const id = $(this).closest('tr').data('id');
                $.post(`/Admin/${module}/Approve/${id}`, {
                    SumTotal: res.value,
                }).then((res) => {
                    if (res.Status) {
                        $(self).fadeOut();
                        swalWithBootstrapButtons.fire('Succeeded!', '', 'success');
                    } else {
                        swalWithBootstrapButtons.fire('Error', '', 'error');
                    }
                });
            }
        });
    });
}

function getMoneyFormat(money) {
    return parseFloat(money).toFixed(2);
}

$(document).ready(function () {
    initShowElement();
    initDeleteElement();
    initBlockElement();
    initReadElement();
    initViewOrder();
    initChangeOrderStatus();
    initAddInputDetail();
    initApproveElement();
});