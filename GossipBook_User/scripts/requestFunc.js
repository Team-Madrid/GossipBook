$(function() {
    if(sessionStorage.getItem('username')) {
        showMenuAndWelcome();
    } else {
        showAnyAndHideOther("welcomeForm");
    }
    
    $("#toHome").on("click", function() {
        showMenuAndWelcome();
    });
    
    $("#logout").on("click", function() {
        sessionStorage.removeItem('objectId');
        sessionStorage.removeItem('username');
        sessionStorage.removeItem('sessionToken');
        sessionStorage.removeItem('fullName');
        
        showAnyAndHideOther("welcomeForm");
    });
    
    $("#toPhonebook").on("click", function() {
        showMenuAndAny("phones");
        
        numberTable();
    });
    
    $("#editPhone").on("click", function() {
        EditNumber(); 
    });

    $("#backToPhonebook").on("click", function() {
        showMenuAndAny("phones");
        
        numberTable();
    });
    $("#toAddPhone").on("click", function() {
        showMenuAndAny("add-phone-form");
    });
    
    $(".toLogin").on("click", function() {
        showAnyAndHideOther("login-form");
    });
    
     $(".toRegister").on("click", function() {
        showAnyAndHideOther("register-form");
    });
    
    $("#addPhone").click(function() {
        var num = $("#addPhoneNumber").val(),
            name = $("#addPersonName").val(),
            data ={
                person: name,
                number: num,
                userId: sessionStorage.getItem("objectId")
            };
        
        addNumber(data);
        
        $("#addPhoneNumber").val("");
        $("#addPersonName").val("");
        
        showMenuAndAny("phones");
    });
});

var url = "http://localhost:26089/";
var resourceUrl = "https://api.parse.com/1/classes/Phone";
var editVariableId;

function showAnyAndHideOther(id) {
    $(".parent").each(function() {
           if($(this).attr("id") !== id) {
               $(this).removeClass("show").addClass("hide");
           } else {
               $(this).removeClass("hide").addClass("show");
           }
        });
}

function showMenuAndAny(id) {
    $(".parent").each(function() {
        if($(this).attr("id") !== id && $(this).attr("id") !== "menu") {
            $(this).removeClass("show").addClass("hide");
        } else {
            $(this).removeClass("hide").addClass("show");
        }
     });
}

function showMenuAndWelcome() {
    showMenuAndAny("welcome");
    
    $("#welcomeMessage").remove();
    
    var $welcomeString = $("<h1 id='welcomeMessage'>Welcome " + sessionStorage.getItem("username") + "</h1>");
    
    $("#welcome").prepend($welcomeString);
}

function login() {
    var success = function(data) {
        sessionStorage.removeItem('objectId');
        sessionStorage.removeItem('username');
        sessionStorage.removeItem('sessionToken');
        sessionStorage.removeItem('fullName');
        
        sessionStorage.setItem('objectId', data['objectId']);
        sessionStorage.setItem('username', data['userName']);
        sessionStorage.setItem('sessionToken', data['access_token']);
        sessionStorage.setItem('fullName', data['fullName']);
        
        showMenuAndWelcome();
        
        showInfoMessage("Login Successful");
    };
    
    var error = function() {
        showInfoMessage("Invalid login");
    };
    
    var username = $("#loginUsername").val(),
        password = $("#loginPassword").val();

        $("#loginUsername").val("");
        $("#loginPassword").val("");

    var data = {
        username: username, 
        password: password,
		grant_type: "password"
		};

    ajaxRequester.post(url + "api/login", data, success, error);
}

function registration() {
    var username = $("#regUsername").val(),
        password = $("#regPassword").val(),
		comfirmPassword = $("#confirmPassword").val(),
        email = $("#regEmail").val();
        
		console.log(username);
		
        $("#regUsername").val("");
        $("#regPassword").val("");
		$("#confirmPassword").val("");
        $("#regEmail").val("");
    
    var success = function(data) {
        $("#loginUsername").val(username);
        $("#loginPassword").val(password);
        
        login();
    };
    
    var error = function() {
        showInfoMessage("Invalid register");
    };
    
    var data = {
        Username: username,
		Email: email,
        Password: password,
        ConfirmPassword: comfirmPassword
		};
        
    ajaxRequester.post(url + "api/Register", data, success, error);
}

function showInfoMessage(msg) {
        noty({
                text: msg,
                type: 'info',
                layout: 'topCenter',
                timeout: 5000}
        );
    }
    

    
    
