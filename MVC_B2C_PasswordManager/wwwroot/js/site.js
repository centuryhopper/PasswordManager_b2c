// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// only allow positive integers to be entered
const ValidateNumber = (event) => {

    var theEvent = event || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    var regex = /[1-9]/

    // console.log(regex.test(key));

    if (!regex.test(key)) {
        theEvent.preventDefault ? theEvent.preventDefault() : (theEvent.returnValue = false);
    }
}
