function Boom()
{
    alert("Boom");
}

function GetEnrolled()
{
    alert("GET TERMS");
    var termSelect = document.getElementById("terms");
    var newOption;

    foreach(terms in Model.terms)
    {
        alert(terms.term_id);
        newOption = document.createElement("option");
        newOption.text = terms.term_id;
        termSelect.add(newOption);
    }
}

var button = document.querySelector('#button');
var button1 = document.querySelector('#button1');

button.addEventListener('click', Boom);
button1.addEventListener('click', GetTerms)
