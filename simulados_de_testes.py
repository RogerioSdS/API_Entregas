import pytest
import requests
from faker import Faker
import concurrent.futures
import time

fake = Faker()
base_url = "http://localhost:5088/api/Auth"

# Função auxiliar para fazer requisição
def make_request(email, token):
    try:
        start_time = time.time()
        response = requests.get(f"{base_url}/ConfirmEmail?token={token}&email={email}", timeout=5)
        elapsed_time = time.time() - start_time
        return {
            "email": email,
            "token": token,
            "status_code": response.status_code,
            "response_text": response.text.strip(),
            "elapsed_time": elapsed_time
        }
    except requests.exceptions.RequestException as e:
        return {
            "email": email,
            "token": token,
            "status_code": None,
            "response_text": str(e),
            "elapsed_time": None
        }

# Teste massivo
def test_confirm_email_massive():
    # Dados reais (substitua com seus dados)
    real_data = [
        {
            "email": "rogerio@rogerio.com",
            "token": "CfDJ8Cy6c9g6YHlAu%2fN9aMFYdTbUxnfP1W7%2bd0YyY2%2fc5Zjz9iBrzglJOLWFoLQcMA7VYYUa8SkgSCCRWgjVB%2fF1omBJ1teEU5qE9Ik6Nu6p67ekMaTo9siEho7DiapqqZZPhvgX7723TOu0CfLH0UaKaBvKq7eHQX0wcAVztZTvXIzDVawmTb8g8BNGhrJ9iqa%2b1w%3d%3d",
            "expected_status": 200,
            # "expected_response": "Email confirmado com sucesso."
        }
    ]

    # Dados gerados (cenários variados)
    generated_data = [
        {"email": fake.email(), "token": fake.uuid4(), "expected_status": 400} ,
        {"email": "rogerio@rogerio.com", "token": "invalid-token", "expected_status": 400} ,
        {"email": fake.email(), "token": "abc123", "expected_status": 400}
    ]

    # Combinar dados reais e gerados
    test_cases = real_data + generated_data * 35  # Repetir dados gerados para aumentar a carga

    # Executar requisições em paralelo
    with concurrent.futures.ThreadPoolExecutor(max_workers=10) as executor:
        results = list(executor.map(lambda x: make_request(x["email"], x["token"]), test_cases))

    # Verificar resultados
    for i, result in enumerate(results):
        expected = test_cases[i]
        print(f"Test {i+1}: Email={result['email']}, Token={result['token'][:20]}..., "
              f"Status={result['status_code']}, Response={result['response_text']}, "
              f"Time={result['elapsed_time']:.2f}s")
        assert result["status_code"] == expected["expected_status"], \
            f"Test {i+1} failed: Expected status {expected['expected_status']}, got {result['status_code']}"
        # assert result["response_text"] == expected["expected_response"], \
        #     f"Test {i+1} failed: Expected response '{expected['expected_response']}', got '{result['response_text']}'"