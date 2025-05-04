


const elem = document.getElementById('message');
document.addEventListener('submit', e => {
    const form = e.target;
    if(form.id == "auth-modal-form") {
        e.preventDefault();
        const login = form.querySelector('[name="AuthLogin"]').value;
        const password = form.querySelector('[name="AuthPassword"]').value;
        if (login.length == 0)
        {
            elem.innerHTML = "login field can't be blank";
        }
        else if (password.length == 0)
        {
            elem.innerHTML = "password filed can't be blank";
        }
        if (login.length > 0 && password.length > 0)
        {
            const credentials = btoa(login + ':' + password);
            fetch("/User/Signin/", {
                method: 'POST',
                headers: {
                    'Authorization': 'Basic ' + credentials
                }
            }).then(r => r.json())
                .then(j => {
                    if (j.status == 200) {
                        window.location.reload();
                    } else {
                        console.log(j);
                        elem.innerHTML = j.message;
                    }
                })
            console.log("Submission stopped");
        }
    }
    if (form.id == "user-registration-form")
    {
        e.preventDefault();
        const login = document.querySelector('[name="user-login"]').value;
        const name = document.querySelector('[name="user-name"]').value;
        const password = document.querySelector('[name="user-password"]').value;
        const repeatPassword = document.querySelector('[name="repeat-password"]').value;
        const phone = document.querySelector('[name="user-phone"]').value;
        const email = document.querySelector('[name="user-email"]').value;
        const country = document.querySelector('[name="user-country"]').value;
        const birthDate = document.querySelector('[name="user-birthDate"]').value
        console.log(login, name, password, repeatPassword, phone, email, country, birthDate);
        // Серж, тут перед тем как фетч делать, нужно добавить проверку заполнения полей

        if (login.length == 0) {
            windows.alert("login field can't be blank")
        }
        if (name.length == 0) {
            window.alert("name field can't be blank")
        }
         if (password.length == 0) {
            window.alert("password field can't be blank")
        }
        if (repeatPassword.length == 0) {
            window.alert("repeat password field can't be blank")
        }
         if (phone.length == 0) {
            window.alert("phone field can't be blank")
        }
         if (email.length == 0) {
            window.alert("email field can't be blank")
        }
         if (country.length == 0) {
            window.alert("country field can't be blank")
        }
         if (birthDate.length == 0) {
            window.alert("birth date field can't be blank")
        }
         if (password != repeatPassword) {
            window.alert("passwords are not the same")
        }
        else {
            fetch("/User/Register", {
                method: 'POST',
                body: new FormData(form)
            }).then(r => r.json())
                .then(j => {
                   if (j.status == 401) {
                        console.log(j.message);
                        window.alert(j.message);
                    }
                   else {
                       window.location.reload();
                       window.alert("registered successfully");
                    }
                });
        }
    }

    if (form.id == "user-edit-form") {
        e.preventDefault();
        const elem = document.querySelector('[data-auth-ua-id]');
        if (!elem) {
            alert('Увійдіть до системи');
            return;
        }
        const uaId = elem.getAttribute('data-auth-ua-id');
        console.log(uaId);

        const login = document.querySelector('[name="user-login"]').value;
        const name = document.querySelector('[name="user-name"]').value;
        const phone = document.querySelector('[name="user-phone"]').value;
        const email = document.querySelector('[name="user-email"]').value;
        const country = document.querySelector('[name="user-country"]').value;
        const avatar = document.querySelector('[name="user-avatar"]').files[0];
        const aboutuser = document.querySelector('[name="user-about"]').value;
        if (login.length == 0) {
            windows.alert("login field can't be blank")
        }
        if (name.length == 0) {
            window.alert("name field can't be blank")
        }
        if (phone.length == 0) {
            window.alert("phone field can't be blank")
        }
        if (email.length == 0) {
            window.alert("email field can't be blank")
        }
        if (country.length == 0) {
            window.alert("country field can't be blank")
        }
        else {
            fetch(`/User/Change?login=${login}&name=${name}
                        &phone=${phone}&email=${email}&country=${country}
                        &avatar=${avatar}&aboutuser=${aboutuser}&uaId=${uaId}`, {
                method: 'PUT',
            }).then(r => r.json()).then(j => {
                if (j.status == 200) {
                    console.log(j.message);
                    alert("changes successfully");
                }
            });
        }
    }
})


