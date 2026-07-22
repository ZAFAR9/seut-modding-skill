# Reference: Physics

← [Back to reference index](README.md)

Space Engineers uses the Havok physics engine (an older 32-bit version).

## General & Coordinate Clusters

To support 64-bit coordinates within a 32-bit physics engine, the game world is split into multiple Havok worlds (clusters). Entities move seamlessly between these clusters. Mod scripts do not need to manage or interact with clusters in almost all cases.

## Model Collision

- Collision shapes are defined in models. For details, see [Models](models-and-modelxml.md).

## Physics Layers & Collision Matrix

Havok uses layers configured to selectively collide or ignore other layers. 

### Matrix Legend
- **Empty**: No interaction/collision.
- **✔**: Always interact.
- **SV**: Server-side / Singleplayer only.
- **CL**: Client-side (multiplayer) only.

| Layer | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 |
| --- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **OpenableSubpartLayer (4)** | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ |
| **StaticGridsSearchCollisionLayer (5)** | | ✔ | | | | | | | | ✔ | | | | | | | | | | | | | | | | | | |
| **TargetDummyLayer (6)** | ✔ | | | | ✔ | | | ✔ | ✔ | | | ✔ | | | ✔ | ✔ | | | | | | | ✔ | ✔ | | | | |
| **BlockPlacementTestCollisionLayer (7)** | ✔ | | | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | | ✔ | ✔ | | ✔ | | ✔ | | ✔ | ✔ | ✔ | | | ✔ | | ✔ | ✔ | |
| **MissileLayer (8)** | ✔ | | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | | ✔ | ✔ | | ✔ | | | | ✔ | ✔ | ✔ | | | ✔ | ✔ | ✔ | ✔ | |
| **NoVoxelCollisionLayer (9)** | ✔ | | | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | | ✔ | ✔ | | ✔ | | ✔ | | SV | ✔ | ✔ | | | ✔ | | ✔ | ✔ | |
| **LightFloatingObjectCollisionLayer (10)** | ✔ | | | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | | | ✔ | ✔ | | ✔ | ✔ | | ✔ | ✔ | ✔ | | ✔ | |
| **VoxelLod1CollisionLayer (11)** | ✔ | | ✔ | | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ |
| **NotCollideWithStaticLayer (12)** | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ |
| **StaticCollisionLayer (13)** | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | | ✔ | | ✔ | | ✔ | ✔ | ✔ | | | ✔ | ✔ | ✔ | ✔ | |
| **CollideWithStaticLayer (14)** | ✔ | | | | | | | ✔ | ✔ | ✔ | | | | ✔ | | | | | | | | | | | ✔ | ✔ | ✔ | |
| **DefaultCollisionLayer (15)** | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | | ✔ | | ✔ | | ✔ | ✔ | ✔ | | | ✔ | ✔ | ✔ | ✔ | |
| **DynamicDoubledCollisionLayer (16)** | ✔ | | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | | ✔ | | ✔ | | | ✔ | ✔ | | | ✔ | ✔ | ✔ | ✔ | |
| **KinematicDoubledCollisionLayer (17)** | ✔ | | | | | | ✔ | ✔ | ✔ | | ✔ | | | ✔ | | | ✔ | | ✔ | ✔ | ✔ | | | ✔ | | ✔ | ✔ | |
| **CharacterCollisionLayer (18)** | ✔ | | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | | ✔ | ✔ | | ✔ | | | | ✔ | ✔ | ✔ | | | ✔ | ✔ | | | |
| **NoCollisionLayer (19)** | ✔ | | ✔ | | | | | ✔ | ✔ | | | | | | | | | | | | | | ✔ | ✔ | | | | |
| **DebrisCollisionLayer (20)** | ✔ | | | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | | | ✔ | ✔ | | ✔ | ✔ | | | ✔ | ✔ | | ✔ | |
| **GravityPhantomLayer (21)** | ✔ | | | | | | ✔ | ✔ | ✔ | | | | | | | | ✔ | ✔ | | ✔ | | ✔ | | ✔ | | | ✔ | |
| **CharacterNetworkCollisionLayer (22)** | ✔ | | | ✔ | ✔ | SV | | ✔ | ✔ | ✔ | | ✔ | | ✔ | ✔ | | | | ✔ | ✔ | ✔ | | | ✔ | ✔ | | ✔ | |
| **FloatingObjectCollisionLayer (23)** | ✔ | | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | | ✔ | |
| **ObjectDetectionCollisionLayer (24)** | ✔ | | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | | ✔ | | ✔ | ✔ | | | | ✔ | ✔ | | ✔ | |
| **VirtualMassLayer (25)** | ✔ | | | | | | | ✔ | ✔ | | | | | | | | | ✔ | | | | | | ✔ | | | ✔ | |
| **CollectorCollisionLayer (26)** | ✔ | | ✔ | | | | ✔ | ✔ | ✔ | | | | | | | ✔ | | | | ✔ | | | ✔ | ✔ | | | ✔ | |
| **AmmoLayer (27)** | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | ✔ | |
| **VoxelCollisionLayer (28)** | ✔ | | | | ✔ | | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | | ✔ | | ✔ | | ✔ | ✔ | ✔ | | | ✔ | ✔ | | ✔ | |
| **ExplosionRaycastLayer (29)** | ✔ | | | ✔ | ✔ | ✔ | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | | | | | | | | | | | | ✔ | ✔ | |
| **CollisionLayerWithoutCharacter (30)** | ✔ | | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | | | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ | |
| **RagdollCollisionLayer (31)** | ✔ | | | | | | | ✔ | ✔ | | | | | | | | | | | | | | | | | | | ✔ |

---

## TL;DR
- Space Engineers uses 32-bit Havok physics with 64-bit coordinate clusters, handled seamlessly by the engine.
- Collisions are defined directly on models via collision shapes.
- Havok physics layers selectively collide based on a 32x32 collision filter matrix.

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/Physics