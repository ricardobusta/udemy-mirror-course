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
- `SyncVars`
  - When written in the server, broadcast changes to all clients
  - `Callback` - Can hook to a method (using nameof(Method) as the hook parameter). Used to run a method in the client when the SyncVar is updated.
- `Command`
  - Called by a client, executed in the server
  - Can bypass server authority with parameter
- `ClientRcp`
  - Method is ran on all clients when called by the server.
- `TargetRcp`
  - Similar to `ClientRcp`, but can restrict who receives the calls.
  - Only targeted clients receives the call. By default, it's the object owner.
- Unity Callback with Authority
  - Callbacks `OnStartAuthority` and `OnStopAuthority`, only execute if the client has authority over the object
- `ClientCallback` attribute limits Unity default callbacks to be run on client only. `ServerCallback` for server.
- Can create spawn points using `NetworkStartPosition` - And assign algorithm (Random or Round Robin) in the network manager.

## Section 2 - RTS Game

### Project Setup and Unit Spawning

#### Mirror
- Player network, with just network identity
- Spawner network object also belongs to a player
- Spawned object uses Spawner connection to give authority to the right client.
- Add spawner object as a new player gets added. conn.identity maps to the player object.

#### Input System
- Add EventSystem to scene for detecting IPointerClickHandler event.
- Update EventSystem script (New Input System)
- Add Physics Raycaster to camera

### Networking

- Code running on the server in a component that also exist in client -> NetworkBehaviour
- Adding prefix Cmd and Rpc to remote methods help a lot on code readability.
- connectionToClient in NetworkBehaviour is a good way to figure out if objects belong to the same user

### Out of course scope notes:

NavMesh generation at runtime: https://learn.unity.com/tutorial/runtime-navmesh-generation

