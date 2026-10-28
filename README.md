# 🛒 E-Commerce API (Refatorado)

Este projeto consiste na refatoração completa de uma API monolítica de e-commerce desenvolvida em **.NET / C#**. O objetivo principal da refatoração foi transformar um código legado (*God Class*) em uma aplicação organizada, extensível e manutenível, seguindo os princípios **SOLID**, boas práticas de **Clean Architecture** e utilizando **Injeção de Dependência (DI)**.

---

## 🚀 Principais Melhorias e Refatorações Realizadas

### 1. Desmembramento do *God Controller* (Princípio da Responsabilidade Única - SRP)
- O `EcommerceController` original possuía responsabilidades misturadas de produtos, estoque, pedidos, pagamento e logística.
- A camada de apresentação foi dividida em controladores especializados com rotas claras:
  - **`ProdutosController`**: CRUD e gerenciamento de catálogo de produtos.
  - **`EstoqueController`**: Consultas e reabastecimento de estoque.
  - **`PedidosController`**: Criação, consulta de pedidos, reprocessamento de pagamento e atualização logística.

### 2. Separação em Camadas e Injeção de Dependência (DI)
- **Camada de Serviços (`Services`)**: Criadas as interfaces `IPedidoService` e `IEstoqueService` com suas respectivas implementações (`PedidoService` e `EstoqueService`).
- As *Controllers* deixaram de ter conhecimento direto do banco de dados (`AppDbContext`) e de implementar regras de negócio, passando a atuar apenas como adaptadores HTTP.
- Registro de dependências e configuração do `AppDbContext` centralizados no `Program.cs`.

### 3. Correção Crítica de Transação e Estoque
- **Garantia de Atomicidade**: Adicionada transação explícita com o Entity Framework (`BeginTransactionAsync`).
- **Correção de Bug de Estoque**: Anteriormente, o estoque era baixado antes da confirmação do pagamento. Caso o pagamento falhasse, o estoque não era estornado. Agora, em caso de falha ou exceção no pagamento, a transação sofre *rollback*, garantindo a integridade dos dados.

### 4. Uso de DTOs (Data Transfer Objects)
- As entidades do banco de dados não são mais expostas diretamente nas requisições e respostas da API.
- Implementação de DTOs como `PedidoRequest`, `PedidoResponseDto` e `ItemPedidoResponseDto`, separando o contrato externo da API do modelo de dados interno e evitando valores nulos indesejados nas respostas.

### 5. Programação Assíncrona (`async/await`)
- Removidas todas as chamadas síncronas bloqueantes (como `.Result` e `.Wait()`).
- Refatoração de todos os fluxos de I/O para utilizar métodos assíncronos do Entity Framework (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, etc.).

### 6. Tratamento de Exceções e Respostas HTTP
- Adicionados blocos `try/catch` estruturados com mensagens de erro adequadas (`400 Bad Request`, `404 Not Found`, `500 Internal Server Error`) e logs controlados.

---

## 🛠️ Tecnologias Utilizadas

* **.NET / C#**
* **Entity Framework Core**
* **SQLite / SQL Server**
* **Swagger / OpenAPI** (para documentação interativa dos endpoints)

---

## 📁 Estrutura do Projeto

```text
EcommercePiorApi/
├── Controllers/
│   ├── EstoqueController.cs
│   ├── PedidosController.cs
│   └── ProdutosController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   ├── ItemPedidoRequest.cs
│   ├── PedidoRequest.cs
├── Enums/
│   ├── FormaPagamento.cs
│   └── StatusPedido.cs
├── Models/
│   ├── ItemPedido.cs
│   ├── Pedido.cs
│   └── Produto.cs
├── Services/
│   ├── EstoqueService.cs
│   ├── IEstoqueService.cs
│   ├── IPedidoService.cs
│   └── PedidoService.cs
└── Program.cs
```

---

## 🔧 Como Executar a Aplicação

1. **Clonar o repositório:**
   ```bash
   git clone https://github.com/amrodrigues/desafio-qualidade-de-codigo-womakerscode.git
   ```

2. **Navegar até a pasta do projeto:**
   ```bash
   cd desafio-qualidade-de-codigo-womakerscode/EcommercePiorApi
   ```

3. **Restaurar dependências e executar:**
   ```bash
   dotnet restore
   dotnet run
   ```

4. **Acessar a documentação do Swagger:**
   Abra o navegador em `https://localhost:<porta>/swagger` para testar os endpoints da API.
