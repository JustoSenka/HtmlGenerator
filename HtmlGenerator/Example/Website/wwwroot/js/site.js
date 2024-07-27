// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/* MODAL IMAGES */

// Get the image and insert it inside the modal - use its "alt" text as a caption
var modal = document.getElementById("modal");
var modalContent = document.getElementById("modal-content");
var modalCaption = document.getElementById("modal-caption");

// When the user clicks on <span> (x), close the modal
var modalClose = document.getElementById("modal-close");
modalClose.onclick = function () {
    modal.style.display = "none";
}

$(function () {
    $('.modal-img').on('click', function (e) {
        modal.style.display = "block";
        modalContent.src = this.src;
        modalCaption.innerHTML = this.alt;
    });
});

