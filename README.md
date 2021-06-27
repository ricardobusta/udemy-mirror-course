# udemy-mirror-course
https://www.udemy.com/course/unity-multiplayer

Learning how to make multiplayer RTS

# Takeaways

## Section 1 - Introduction

- How network manager works
- What are network identities
- Authority
  - By default, server has authority. A client can obtain authority over an object. Then that client is able to request changes to that object state.
  - Assigned during spawn or with AssignClientAuthority method
- SyncVars
  - When written in the server, broadcast changes to all clients
  - Callback - Can hook to a method (using nameof(Method) as the hook parameter). Used to run a method in the client when the SyncVar is updated.  
- Command
  - Called by a client, executed in the server
  - Can bypass server authority with parameter
- ClientRcp
  - Method is ran on all clients when called by the server.
- TargetRcp
  - Similar to ClientRcp, but can restrict who receives the calls.
  - Only targeted clients receives the call. By default, it's the object owner.
- Unity Callback with Authority
  - Callbacks OnStartAuthority and OnStopAuthority, only execute if the client has authority over the object
- Can create spawn points using NetworkStartPosition - And assign algorithm (Random or Round Robin) in the network manager.

## Section 2 - RTS Game

