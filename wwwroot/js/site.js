// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const menu = document.getElementById("menu");
Array.from(document.getElementsByClassName("menu-item")).forEach((item, Index) => {
    item.onmouseover = () => {
        menu.dataset.activateIndex - Index;
    }
});
