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
        console.log(form)
        // Серж, тут перед тем как фетч делать, нужно добавить проверку заполнения полей
        fetch("/User/Register", {
            method: 'POST',
            body: new FormData(form)
        }).then(r => r.json())
            .then(j => {
                if (j.status == 401) {
                    console.log(j.message);
                    window.alert("registered successfully");
                } else {
                    window.location.reload();
                }
            });
    }
})
