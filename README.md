# traffic-api

Sample project to demonstrate an MCP server in AspNetCore.

The solution contains 3 projects:
- web api (with openapi support and proxy for verkeerscentrum.be)
- API client library (with client to consume Traffic API)
- mcp server (using ModelContextProtocol.AspNetCore)

```mermaid
graph TD
    subgraph "Traffic.API.sln"
        subgraph "Traffic.API Project"
            API["Traffic.API"] --> TrafficCtrl["TrafficController.cs"]
            API --> TrafficDto["TrafficDto.cs"]
            API --> TrafficProxy["TrafficProxy.cs"]
        end
        
        subgraph "Traffic.APIClient Project"
            APIClient["Traffic.APIClient"] --> IClient["ITrafficClient.cs"]
            APIClient --> ClientImpl["TrafficClient.cs"]
        end

        subgraph "Traffic.MCP Project"
            MCP["Traffic.MCP"] --> MCPTool["TrafficTool.cs"]            
        end
        
        APIClient --> API
        MCP --> APIClient
    end

    subgraph "MCP Clients"
        VSCode["VS Code"] --> MCP
        Claude["Claude Desktop"] --> MCP
    end
      API --> ExternalAPI["verkeerscentrum.be<br/>(External API)"]
    
    classDef project fill:#f9f,stroke:#333,stroke-width:2px;
    classDef file fill:#bbf,stroke:#333,stroke-width:1px;
    classDef folder fill:#ddf,stroke:#333,stroke-width:1px;
    classDef consumer fill:#afd,stroke:#333,stroke-width:2px;
    classDef external fill:#fda,stroke:#333,stroke-width:2px;
    classDef boldProject fill:#f9f,stroke:#000,stroke-width:4px;
    
    class APIClient project;
    class API,MCP boldProject;
    class TrafficCtrl,TrafficDto,TrafficProxy,MCPTool,IClient,ClientImpl file;
    class VSCode,Claude consumer;
    class ExternalAPI external;
```

# vscode config 

```
"mcp": 
{
    "inputs": [],
    "servers": {            
        "traffic-mcp-server": {
            "type": "sse",
            "url": "http://localhost:5238/sse"
        }
    }
},
```

# claude desktop config

```
{
    "mcpServers": {
      "traffic": {
        "command": "npx",
        "args": [
          "mcp-remote",
          "http://localhost:5238/sse"
        ]
      }
    }
  }
```

# sample conversation

## Question 1: Claude - "Tell me something about the traffic"

![question 1](img/image-0.png)

## Question 2: Claude - "What is the current traffic in Flanders?"

![alt text](img/image-1.png)

## Question 3: Claude - "Is there still a lot of traffic?"

![alt text](img/image-2.png)

## Question 4: Claude - "Can you check again?"

![alt text](img/image-3.png)

## Question 5: Claude - "Any improvements?"

![alt text](img/image-4.png)

## Question 6: VS Code - "What is the current traffic like?"

![alt text](img/image-5.png)

## Question 7: VS Code - "How is the traffic evolving?"

![alt text](img/image-6.png)

## Question 8: VS Code - "Check again"

![alt text](img/image-7.png)

