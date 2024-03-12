/** Запрос на авторизацию */
async function loginUser() {
    const form = document.querySelector("#accountData");
    const user = serializeForm(form);

    const response = await fetch("api/user/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(user)
    });

    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const res = await response.json();

        // установка токена и инфо пользователя
        sessionStorage.setItem(tokenKey, res.token);
        sessionStorage.setItem(loginKey, res.userName);
        setClearForm("accountData");
        await updatePage();
    }
    // обработка ошибок
    else if (response.status == 401 || response.status == 400) {
        const err =  await response.json();
        document.getElementById("loginError").style.display = "flex";
        document.getElementById("loginError").textContent = err;
    }
}


/** получение данных таблицы пользователей */
async function getUsers() {
    const token = sessionStorage.getItem(tokenKey);
    
    const response = await fetch(getUsersUrl, {
        method: "GET",
        headers: {
            'Accept': "application/json",
            'Authorization': `Bearer ${token}`
        }
    });

    const rows = document.getElementById("usersTable");
    rows.innerHTML = "";

    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const users = await response.json();

        console.log(users);
        showTable();
        // добавляем полученные элементы в таблицу
        users.forEach(user => rows.append(setRowData(user)));
    }
    // если 401  - возвращаем авторизацию
    else if (response.status === 401) {
        showAccount();
        updateUserInfo();
    }
}

/** получение данных для редактирования */
async function getUser(id) {
    showUserModal();
    const token = sessionStorage.getItem(tokenKey);
    const response = await fetch(getUserByIdUrl.replace("--1", id), {
        method: "GET",
        headers: {
            'Accept': "application/json",
            'Authorization': `Bearer ${token}`
        }
    });

    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const user = await response.json();
        setFormData('userData', user);
    }
    else if (response.status === 401) {
        updateUserInfo();
        showAccount();
    }
}

/** сохранение данных пользователя */
async function saveUser() {
    const form = document.querySelector("#userData");
    const user = serializeForm(form);
    const token = sessionStorage.getItem(tokenKey);

    const response = await fetch(saveUserUrl, {
        method: "POST",
        headers: {
            'Accept': "application/json",
            'Content-Type': 'application/json;charset=utf-8',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(user),
    });

    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        hideUserModal();
        setClearForm("userData");
        await getUsers();
    }

    // при ошибке авторизации возвращаемся к авторизации 
    else if (response.status === 401) {
        rows.innerHTML = "";
        updateUserInfo();
        showAccount();
    }
    // при ошибках 400 выводим текст
    else if (response.status === 400) {
        const err = await response.json();

        console.log(err);

        document.getElementById("saveError").style.display = "flex";
        document.getElementById("saveError").textContent = err;
    }
}

/** удаление пользователя */

async function deleteUser(id) {

    const token = sessionStorage.getItem(tokenKey);

    const response = await fetch(deleteUserUrl.replace("--1", id), {
        method: "GET",
        headers: {
            'Accept': "application/json",
            'Authorization': `Bearer ${token}`
        }
    });

    // если запрос прошел нормально
    if (response.ok === true) {
        await getUsers();
    }
    else if (response.status === 401) {
        showAccount();
    }
}

async function updatePage() {
    await getUsers();
    updateUserInfo();
}

/** скрыть поля ошибок */
function hideAccountError() {
    document.getElementById("loginError").style.display = "none";
    document.getElementById("loginError").textContent = '';
}

function hideSaveError() {
    document.getElementById("saveError").style.display = "none";
    document.getElementById("saveError").textContent = '';
}

/** переход к авторизации */
function showAccount() {
    document.getElementById("accountPart").style.display = "flex";
    document.getElementById("contentPart").style.display = "none";
    hideAccountError();
}

/** переход к таблице */
function showTable() {
    document.getElementById("accountPart").style.display = "none";
    document.getElementById("contentPart").style.display = "flex";
}

/** переход к окну редактирования */
function showUserModal() {
    document.getElementById("backModal").style.display = "flex";
    document.getElementById("editModal").style.display = "flex";
    hideSaveError();
}

/** скрытие окна редактирования */
function hideUserModal() {
    document.getElementById("backModal").style.display = "none";
    document.getElementById("editModal").style.display = "none";
}

/** обновление данных в хедере */
function updateUserInfo() {
    const loginName = sessionStorage.getItem(loginKey);

    console.log(loginName);

    if (loginName) {
        const userDiv = document.createElement('div');
        const userLabel = document.createElement('label');
        userLabel.append(`Вы вошли как ${loginName}`);
        userDiv.append(userLabel);
        const logoutButton = document.createElement('button');
        logoutButton.setAttribute("class", 'logout-btn');
        logoutButton.addEventListener("click", () => logOut());
        logoutButton.append("X");
        userDiv.append(logoutButton);
        document.getElementById("accountInfo").append(userDiv);
    }
    else {
        document.getElementById("accountInfo").innerHTML = "";
    }
}

/** "разлогигивание" */
function logOut() {
    sessionStorage.setItem(tokenKey, '');
    sessionStorage.setItem(loginKey, '');
    updatePage();
}

/** создание строки для таблицы*/
function setRowData(user) {

    const rowDiv = document.createElement("div");
    rowDiv.setAttribute("class", "table-grid-row");

    //** Логин */
    const loginDiv = document.createElement("div");
    loginDiv.setAttribute("class", "table-row");
    loginDiv.append(user.login);
    rowDiv.append(loginDiv);

    //** Имя */
    const nameDiv = document.createElement("div");
    nameDiv.setAttribute("class", "table-row");
    nameDiv.append(user.fullName);
    rowDiv.append(nameDiv);

    /** Почта */
    const mailDiv = document.createElement("div");
    mailDiv.setAttribute("class", "table-row");
    mailDiv.append(user.email);
    rowDiv.append(mailDiv);

    /** Дата рождения */
    const dateDiv = document.createElement("div");
    dateDiv.setAttribute("class", "table-row");
    dateDiv.append(user.birthDate);
    rowDiv.append(dateDiv);

    //** Элемент с кнопками */
    const buttonsDiv = document.createElement("div");
    buttonsDiv.setAttribute("class", "table-row");

    const editLink = document.createElement("button");
    editLink.setAttribute("class", "table-button");
    editLink.append("Изменить");
    editLink.addEventListener("click", async () => await getUser(user.id));
    buttonsDiv.append(editLink);

    const removeLink = document.createElement("button");
    removeLink.setAttribute("class", "table-button");
    removeLink.append("Удалить");
    removeLink.addEventListener("click", async () => await deleteUser(user.id));
    buttonsDiv.append(removeLink);

    rowDiv.appendChild(buttonsDiv);

    return rowDiv;
}

/** сериализация формы */
function serializeForm(form) {
    const { elements } = form;
    const data = Array.from(elements)
        .filter((item) => !!item.name)
        .map((element) => {
            const { name, value } = element;
            return { name, value }
        })

    const result = {};

    data.forEach((item) => {
        result[item.name] = item.value;
    })

    return result;
}

/** установка данных формы */
function setFormData(formName, user) {
    const form = document.getElementById(formName);
    const inputs = form.getElementsByTagName('input');

    for (let input of inputs) {
        input.value = user[input.name];
    }
}

/** очистка формы */
function setClearForm(formName) {
    const form = document.getElementById(formName);
    const inputs = form.getElementsByTagName('input');

    for (let input of inputs) {
        input.value = "";
    }
}