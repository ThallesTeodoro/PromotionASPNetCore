// import $ from '~/lib/jquery/dist/jquery.min.js';

const uri = 'https://localhost:5001/api/newsletter/register';
const uriContact = 'https://localhost:5001/api/contact';

function register() {
  const form = document.querySelector('[data-form]');
  
  const item = {
    name: form.elements[0].value.trim(),
    email: form.elements[1].value.trim()
  };

  fetch(uri, {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  })
  .then(response => response.json())
  .then(() => {
    showResponse('success', 'Registrado com sucesso');
  })
  .catch(error => showResponse('error', 'Erro ao enviar o formulário'));
}

function showResponse(type, message)
{
  const responseElement = document.querySelector('[data-response]');
  responseElement.classList.remove('none');
  responseElement.innerHTML = message;

  if (type == 'error') {
    responseElement.classList.add('text-danger');
  }

  const form = document.querySelector('[data-form]');
  form.elements[0].value = '';
  form.elements[1].value = '';
}

// contact
function contact() {
  const form = document.querySelector('[data-form-contact]');
  
  const item = {
    profile: form.elements[1].value.trim(),
    message: form.elements[2].value.trim()
  };

  fetch(uriContact, {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  })
  .then(response => response.json())
  .then(() => {
    showContactResponse('success', 'Mensagem enviada com sucesso');
  })
  .catch(error => showContactResponse('error', 'Erro ao enviar o formulário'));
}

function showContactResponse(type, message)
{
  const responseElement = document.querySelector('[data-contact-response]');
  console.log(type);
  responseElement.classList.remove('none');
  responseElement.innerHTML = message;

  if (type == 'error') {
    responseElement.classList.add('text-danger');
  } else {
    responseElement.classList.add('text-success');
  }

  const form = document.querySelector('[data-form]');
  form.elements[0].value = '';
  form.elements[1].value = '';
}