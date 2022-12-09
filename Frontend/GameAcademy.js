// Global variables 
var Elements = ["home", "register", "login", "shop", "guest", "play", "bag"];
var BagItems = {};
var NotLoggedInStatusMessage = "Register and Login to fully enjoy our website";
var LoggedInStatusMessage = "Logged in as ";

// method to dynamically toggle between pages 
const TogglePages = () => {
    for (let i = 0; i < Elements.length; i++) {
        let button = Elements[i] + "-button";
        if (Elements[i] == sessionStorage.getItem("currentPage")) {
            document.getElementById(Elements[i]).style.display = 'block'; 
            document.getElementById(button).className = "enabled";
        } else {
            document.getElementById(Elements[i]).style.display = 'none'; 
            document.getElementById(button).className = "notEnabled";
        }
    } 
}

// Home methods 
const ourHome= () => {
    sessionStorage.setItem("currentPage", "home");
    document.getElementById("home").innerHTML = `<p>
    Welcome to The Game Academy - the number 1 premium chess website.<br>
    Get started by taking a few seconds to get registered and logged in to purchase our exclusive chess products,
    playing chess against other real players in real-time and before you leave, please leave a comment in our guest book.
    </p>
    <img id='chessImg' src='chess2.jpeg'/>`
    TogglePages();
}


// Bag methods 
const ourBag = () => {
    sessionStorage.setItem("currentPage", "bag");
    TogglePages();

    const items = JSON.parse(window.sessionStorage.getItem("BagItems"));

    var bagTitle = document.getElementById("bag-title");
    if (window.sessionStorage.getItem("username") === null) {
        ourLogin();
    } else if (Object.keys(items).length === 0) {
        bagTitle.innerText = "Bag is empty";
    } else {
        bagTitle.innerText = "Bag Items";
        document.getElementById("bag-button").innerHTML = '<i class="bi bi-bag-fill fa-4x" onclick="ourBag()"></i>';
    }
    
    let htmlString = "";
    const bag = document.getElementById("bag");
    for(var key in items) {
        htmlString += `<tr id="${key}"><td><img src="https://localhost:5000/api/ItemPhoto/${key}" class="bag-items"/></td>
        <td>
        <label for="quantity">Quantity:</label>
        <input type="number" id="bag-quantity" name="quantity" min="1" value="${items[key]}">
        <button type="button" id="remove-item" onClick='removeItem(${key})'>Remove item</button
        </td><tr>`;
    }
    const ourTable = document.getElementById("bag-table");
    ourTable.innerHTML = htmlString;
}

const removeItem = (id) => {
    const items = JSON.parse(window.sessionStorage.getItem("BagItems"));
    delete items[id];
    if (Object.keys(items).length === 0) {
        document.getElementById("bag-title").innerText = "Bag is empty";
        document.getElementById("bag-button").innerHTML = '<i class="bi bi-bag fa-4x" onclick="ourBag()"></i>';
    }
    window.sessionStorage.setItem("BagItems", JSON.stringify(items));
    var bagTable = document.getElementById("bag-table");
    for (var i = 0; i < bagTable.rows.length; i++){
        if (bagTable.rows[i].id == id) {
            bagTable.deleteRow(i);
        }
    }
}


// Shop methods
const ourShop = () => {
    sessionStorage.setItem("currentPage", "shop");
    TogglePages();

    const fetchPromise = fetch('https://localhost:5000/api/AllItems',
    {
        method: "GET",
        headers : {
            "Accept" : "application/json",
        },
    });
    const streamPromise = fetchPromise.then((response) => response.json());
    streamPromise.then((data) => showDetails(data));
    const showDetails = (products) => {
        let htmlString = "<tr>";
        const showProduct = (product) => {
            htmlString += `<td><img src="https://localhost:5000/api/ItemPhoto/${product.id}" class="products" 
            onClick='showProductInfo(${JSON.stringify(product)})'><h3>${product.name}</h3></td>`;
        }
        products.forEach(showProduct);
        const ourTable = document.getElementById("ourtable");
        htmlString += "</tr>";
        ourTable.innerHTML = htmlString;
    }
}

const showProductInfo = (product) => {
    const modal = document.getElementById("product-popup");
    const modalHeader = document.getElementById("product-header");
    const modalBody = document.getElementById("product-body");
    modalHeader.innerHTML = `<h1>${product.name}</h1>`;
    modalBody.innerHTML = `<img src="https://localhost:5000/api/ItemPhoto/${product.id}" class="imgPopUp"/>
    <h1>$${product.price}</h1>
    <label for="quantity">Quantity:</label>
    <input type="number" id="quantity" name="quantity" min="1" value="1">
    <button type="button" id="add-to-bag" class="notAdded" onClick='addToBag(${JSON.stringify(product)})'>Add to Bag</button>
    <table class="popupTable">
        <tr><td><strong>Product ID:</strong></td><td> ${product.id}</td></tr>
        <tr><td><strong>Description:</strong></td><td> ${product.description}</td></tr>
    </table>`;
 
    modal.style.display = "block";
    window.onclick = function(event) {
        if (event.target == modal) {
            modal.style.display = "none";
            modalHeader.innerHTML = "";
            modalBody.innerHTML = "";
        }
    }
}

const addToBag = (product) => {
    if (sessionStorage.getItem("username") !== null) {
        var items = JSON.parse(window.sessionStorage.getItem("BagItems"));
        items[product.id] = document.getElementById("quantity").value;
        window.sessionStorage.setItem("BagItems", JSON.stringify(items));
        document.getElementById("bag-button").innerHTML = '<i class="bi bi-bag-fill fa-4x" onclick="ourBag()"></i>';
        document.getElementById("add-to-bag").innerText = "Added to Bag";
        document.getElementById("add-to-bag").style.backgroundColor = "brown";
        document.getElementById("add-to-bag").style.color = "burlywood";
    } else {
        const modal = document.getElementById("product-popup");
        modal.style.display = "none";
        ourLogin();
    }
}

const search = () => {
    if (document.getElementById("search").value == "") {
        ourShop();
    } else {
        const fetchPromise = fetch('https://localhost:5000/api/GetItems/' + document.getElementById("search").value,
        {
            method: "GET",
            headers : {
                "Accept" : "application/json",
            },
        });
        const streamPromise = fetchPromise.then((response) => response.json());
        streamPromise.then((data) => showDetails(data));
    }
}


// Register and Log(in)/out methods
const ourRegister = () => {
    sessionStorage.setItem("currentPage", "register");
    TogglePages();
    document.getElementById("register-msg").innerText = ''; 
    document.getElementById("register-form").reset();   
}

const ourLogin = () => {
    sessionStorage.setItem("currentPage", "login");
    TogglePages();
    document.getElementById("login-msg").innerText = ''; 
    document.getElementById("login-form").reset();    

    window.sessionStorage.setItem("BagItems", JSON.stringify(BagItems));
}

const ourLogout = () => {
    document.getElementById("login-status").innerText = NotLoggedInStatusMessage;
    document.getElementById("logout-button").style.display = 'none';
    document.getElementById("login-button").style.display = 'block';
    document.getElementById("bag-button").innerHTML = '<i class="bi bi-bag fa-4x" onclick="ourBag()"></i>';
    sessionStorage.clear();
    ourGuest();
}

const Register = () => {
    const fetchPromise = fetch('https://localhost:5000/api/Register',
    {
        headers : {
            "Accept" : "text/plain",
            "Content-Type" : "application/json",
        },
        method : "POST",
        body: JSON.stringify({
            username : document.getElementById("register-form").username.value,
            password :  document.getElementById("register-form").password.value,
            email : document.getElementById("register-form").email.value
        })
    });
    const streamPromise = fetchPromise.then((response) => response.text());
    streamPromise.then((data) => {
        if (data !== "Username not available.") {
            ourLogin();
        } else {
            document.getElementById("register-msg").innerText = data;
        }
    });
}

const Login = () => {
    const fetchPromise = fetch('https://localhost:5000/api/GetVersionA',
    {
        headers : {
            "Authorization" : "Basic " + btoa(document.getElementById("login-form").username.value + ":" + 
            document.getElementById("login-form").password.value)
        }
    })
    .then(function(response) {
        if (response.status == 200) {
            document.getElementById("login-msg").innerText = "Succesfully logged in";
            sessionStorage.setItem("username", document.getElementById("login-form").username.value);
            sessionStorage.setItem("password", document.getElementById("login-form").password.value);
            document.getElementById("login-status").innerText = LoggedInStatusMessage +  document.getElementById("login-form").username.value;
            document.getElementById("logout-button").style.display = 'block';
            document.getElementById("login-button").style.display = 'none';
            ourShop();
        } else {
            document.getElementById("login-msg").innerText = "Wrong username and/or password, please re-enter valid credentials";
        };
    })

}


// Guest Book methods 
const ourGuest = () => {
    sessionStorage.setItem("currentPage", "guest");
    TogglePages();

    const fetchPromise = fetch('https://localhost:5000/api/GetComments',
    {
        method: "GET",
        headers : {
            "Accept" : "application/json",
        },
    });
    const streamPromise = fetchPromise.then((response) => response.json());
    streamPromise.then((data) => showComments(data));

    const showComments = (comments) => {
        var display = document.getElementById("guest-comments");
        display.contentWindow.document.open()
        for (var i = 0; i < comments.length; i++) {
            display.contentDocument.write(`${comments[i].userComment} - <strong>${comments[i].name}</strong> ${comments[i].time}<br><br>`);
        }
    }
}

const postComment = () => {
    if (window.sessionStorage.getItem("username") !== null) {
        const fetchPromise = fetch('https://localhost:5000/api/WriteComment',
        {
            method : "POST",
            headers : {
                "Content-Type" : "application/json",
            },
            body: JSON.stringify({
                userComment : document.getElementById("comment-text").value,
                name :  window.sessionStorage.getItem("username")
            })
        })
        .then(function(response) {
            window.location.reload();
        });
    } else {
        ourLogin();
    }
}


// Chess Methods 
const ourPlay = () => {
    sessionStorage.setItem("currentPage", "play");
    TogglePages();
}

const PairUp = () => {
    if (window.sessionStorage.getItem("username") !== null) {
        const userName = sessionStorage.getItem('username');
        const password = sessionStorage.getItem('password');
        const fetchPromise = fetch('https://localhost:5000/api/PairMe',
        {
            method : "GET",
            headers : {
                "Authorization" : "Basic " + btoa(userName + ":" + password),
                "Accept" : "application/json",
            },
        });
        const streamPromise = fetchPromise.then((response) => response.json());
        streamPromise.then((data) => update(data));
        const update = (info) => {
            if (info !== null) {
                sessionStorage.setItem('id', info.gameId);
                if (info.state == "wait") {
                    document.getElementById("quit").style.display = 'block';
                    document.getElementById("startText").innerText = `Wait for another player to join. 
                    Check 'Try Game' intermittently to see if someone paired up with you. Please do not spam.`;
                } else {
                    let opponent;
                    let color;
                    if (sessionStorage.getItem('username') == info.player1) {
                        color = "white";
                        opponent = info.player2;
                    } else {
                        color = "black";
                        opponent = info.player1;
                    }
                    if (color == "white") {
                        document.getElementById("sendMove").style.display = 'block';
                        document.getElementById("startText").innerText = `Great ${sessionStorage.getItem('username')}, 
                        you are playing with ${opponent}. Your pieces are ${color} and the first turn is yours. Good luck!`;
                    } else {
                        document.getElementById("getmove").style.display = 'block';
                        document.getElementById("startText").innerText = `Great ${sessionStorage.getItem('username')}, 
                        you are playing with ${opponent}. Your pieces are ${color} and you are going second. Good luck!`;
                    }
                    document.getElementById("quit").style.display = 'block';
                    document.getElementById("start").style.display = 'none';

                }
            }
        }
    } else {
        ourLogin();
    }
}

const GetTheirMove = () => {
    if (window.sessionStorage.getItem("username") !== null) {
        const userName = sessionStorage.getItem('username');
        const password = sessionStorage.getItem('password');
        const fetchPromise = fetch('https://localhost:5000/api/TheirMove' + "/" + sessionStorage.getItem('id'),
        {
            method : "GET",
            headers : {
                "Authorization" : "Basic " + btoa(userName + ":" + password),
                "Accept" : "text/plain",
            },
        });
        const streamPromise = fetchPromise.then((response) => response.text());
        streamPromise.then((data) => checkMove(data));
        const checkMove = (move) => {
            if (move == "" || move == null) {
                document.getElementById("status").style.display = 'block';
                document.getElementById("status").innerText = "Waiting for opponent's move";
            } else {
                document.getElementById("sendMove").style.display = 'block';
                document.getElementById("getmove").style.display = 'none';
            }
        }
    } else {
        ourLogin();
    }
}

const SendMove = () => {
    if (window.sessionStorage.getItem("username") !== null) {
        const userName = sessionStorage.getItem('username');
        const password = sessionStorage.getItem('password');
        const fetchPromise = fetch('https://localhost:5000/api/MyMove',
        {
            method : "POST",
            headers : {
                "Authorization" : "Basic " + btoa(userName + ":" + password),
                "Content-Type" : "application/json",
            },
            body: JSON.stringify({
                gameId : sessionStorage.getItem('id'),
                move : "move chess piece"
            })
        })
        const streamPromise = fetchPromise.then((response) => response.text());
        streamPromise.then((data) => {
            if (sessionStorage.getItem('move') === "yes") {
                document.getElementById("getmove").style.display = 'block';
                document.getElementById("sendMove").style.display = 'none';
                sessionStorage.setItem('move', "no");
            } else {
                alert("you have not made your move!");
            };
        })
    } else {
        ourLogin();
    }
}

const QuitGame = () => {
    if (window.sessionStorage.getItem("username") !== null) {
        const userName = sessionStorage.getItem('username');
        const password = sessionStorage.getItem('password');
        const fetchPromise = fetch('https://localhost:5000/api/QuitGame' + "/"  + sessionStorage.getItem('id'),
        {
            method : "GET",
            headers : {
                "Authorization" : "Basic " + btoa(userName + ":" + password),
                "Accept" : "text/plain",
            },
        });
        window.location.reload();
    } else {
        ourLogin();
    }
}

const mydragstart = (ev) => {
    ev.dataTransfer.setData("text/plain", ev.target.id);
    alert(ev.target.parentElement.id);
}

const mydragover = (ev) => {
    ev.preventDefault();
}

const mydrop = (ev) => {
    if (ev.dataTransfer !== null) {
        const data = ev.dataTransfer.getData("text/plain");
        ev.target.appendChild(document.getElementById(data));
        sessionStorage.setItem('move', "yes");
        alert(document.getElementById(data).parentElement.id);
    }
}


// persist bag fill on page refresh
const bagStatus = () => {
    const items = JSON.parse(window.sessionStorage.getItem("BagItems"));
    if (items !== null && Object.keys(items).length !== 0) {
        document.getElementById("bag-button").innerHTML = '<i class="bi bi-bag-fill fa-4x" onclick="ourBag()"></i>';
    } 
}

// persist logged in message after page refresh
const loginStatus = () => {
    if (sessionStorage.getItem("username") !== null) {
        document.getElementById("login-status").innerText = LoggedInStatusMessage +  sessionStorage.getItem("username");
        document.getElementById("logout-button").style.display = 'block';
        document.getElementById("login-button").style.display = 'none';
    } 
}
 // persist current page after page refresh
const currentPage = () => {
    if (sessionStorage.getItem("currentPage") === "register") {
        ourRegister();
    } else if (sessionStorage.getItem("currentPage") === "play") {
        ourPlay();
    } else if (sessionStorage.getItem("currentPage") === "guest") {
        ourGuest();
    } else if (sessionStorage.getItem("currentPage") === "login") {
        ourLogin(); 
    } else if (sessionStorage.getItem("currentPage") === "shop") {
        ourShop();
    } else if (sessionStorage.getItem("currentPage") === "bag") {
        ourBag();
    } else {
        ourHome();
    }
}

// maintain version number on footer
const version = () => {
    const fetchPromise = fetch('https://localhost:5000/api/GetVersion',
    {
        method: "GET",
        headers : {
            "Accept" : "text/plain",
        },
    });
    const streamPromise = fetchPromise.then((response) => response.text() );
    streamPromise.then( (data) => document.getElementById("version").innerText = "Version: " + data);
}

version();
currentPage();
loginStatus();
bagStatus();