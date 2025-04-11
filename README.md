# traffic-api

Sample project to demonstrate an MCP server in AspNetCore.

The solution contains 3 projects:
- web api (with openapi support)
- shared components (with proxy for verkeerscentrum.be)
- mcp server (using ModelContextProtocol.AspNetCore)

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

## Question 1: "Tell me something about the traffic"

![question 1](img/image-0.png)

## Question 2: "What is the current traffic in Flanders?"

![alt text](img/image-1.png)

## Question 3: "Is there still a lot of traffic?"

![alt text](img/image-2.png)

## Question 4: "Can you check again?"

![alt text](img/image-3.png)

## Question 5: "Any improvements?"

![alt text](img/image-4.png)