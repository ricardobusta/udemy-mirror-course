# udemy-mirror-course
https://www.udemy.com/course/unity-multiplayer

Learning how to make multiplayer RTS

Instructor Youtube Channel: https://www.youtube.com/c/DapperDinoCodingTutorials

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
- Network manager has a `singleton` access.
- When implementing a `lobby`, we need to remove the `Offline Scene` and `Online Scene` references from network manager.
  - This breaks everything. You need to spawn things (except the player) manually after changing to the proper scene.
- When overriding some NetworkManager methods, we `MUST` call the base method. I did for only some, and forgot for `OnServerConnect`. This made the server stop spawning player game objects.
- The instructor made the camera a NetworkBehaviour. For the game we do in the course, it could totally be a local cotntrolled object, but it might be useful for an actual RTS if we want to be able to control the client camera from the server side.

#### Authority

- Also has OnStartAuthority and OnStopAuthority callbacks
  - Called on clients that have authority over that object when you get authority or lose authority over it

#### Steam Transport 

- https://mirror-networking.gitbook.io/docs/transports/
  - https://mirror-networking.gitbook.io/docs/transports/fizzysteamworks-transport

### Major Issues

- I had this misconception of trying to get the player that owned certain object on client. However this is more like a server-side logic. So instead I had to make a network behaviour get the value server side and then only set the value to a syncvar to the clients. E.g. team color.
  - Idea for this: Teams should have material for their colors. Then each unit only gets the team id from the server side, and fetches the material locally.

### Extra

- TIL RectTransformUtility is nice
- Looks very similar with https://docs-multiplayer.unity3d.com/, need to experiment with it sometime.

### Out of course scope notes:

- NavMesh generation at runtime: https://learn.unity.com/tutorial/runtime-navmesh-generation


