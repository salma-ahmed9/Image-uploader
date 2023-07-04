const titleinput = document.getElementById("image-title");
const imageinput = document.getElementById("image-file");
const submitBtn = document.getElementById('submit-btn');
const form = document.getElementById("myform");
form.addEventListener('submit', function (event) {
    if (imageinput.value === '' && titleinput.value === '') {
        event.preventDefault();
        imageinput.style.borderColor = 'red';
        imageinput.classList.add('error');
        imageinput.nextElementSibling.textContent = "please choose an image";
        titleinput.style.borderColor = 'red';
        titleinput.classList.add('error');
        titleinput.nextElementSibling.textContent = "please enter a title for the image";
    }
    else if (titleinput.value === '') {
        event.preventDefault(); // prevent the form submission
        titleinput.style.borderColor = 'red';
        titleinput.classList.add('error');
        titleinput.nextElementSibling.textContent = "please enter a title for the image";
    }
    else if (imageinput.value === '') {
        event.preventDefault();
        imageinput.style.borderColor = 'red';
        imageinput.classList.add('error');
        imageinput.nextElementSibling.textContent = "please choose an image";
    }
});
titleinput.addEventListener('input', function (event) {
    titleinput.style.borderColor = 'black';
    titleinput.nextElementSibling.textContent = "";
    

});

imageinput.addEventListener('input', function (event) {
    imageinput.style.borderColor = 'black';
    imageinput.nextElementSibling.textContent = "";
});