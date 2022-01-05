### ATTENTION
This project is open so others can learn from it.<br>
I do not feel it is ready for public use, so I don't sugggest using it in the current state.<br>
Many things can change, since I'm still experimenting with ideas.<br>

If you have any suggestions or ideas, feel free to open an issue about them.

---

`tcc-sniffer` is a program that collects and decodes network packets relating to Albion Online.<br>
This program does not collect or decode any data that would break the [`game rules`](https://albiononline.com/en/game-rules).<br>
The program will never send any data collected or decoded to anything outside your computer.<br>

The program will run a simple tcp server @ localhost:9999 by default.<br>
For those unaware, localhost is a special ip that only your computer has access to.<br>
You can find a detailed example using nodejs in [`/examples/`](/examples/client-nodejs).<br>
This example runs the sniffer in the background, connects to the server, and consumes the data.<br>

<!--
If you want to consume the data as a third-party, please consider building your tool in the [`tcc-client`](). )
You can head over to [`tcc-extension-template`]() to learn how to do that.
-->

---

## What does the program collect and decode?
<details>
- Chat Messages <br>
- Silver / Gold Amounts <br>
</details>

This list will continue to expand as I get to things...

## How to build the executable?
I have only used this on Windows. Linux/Mac users will have to figure things out themselves.

Requirements(Windows):
- Install [`NPCAP`](https://nmap.org/npcap/) (Look for the `Npcap 1.60 installer` link.)
- Visual Studio (2019 or 2022)

1. Open the solution file in Visual Studio.
2. Select Build from the menubar, then select Publish.
3. Click the Publish button.

## How do Albion's packets work?
There are 3 types of packets; Events, Requests, and Responses.<br>

Events are essentially actions that change something.<br>
Requests are asking for something, or just sending something.<br>
Responses are answering a request.<br>

Packets are identified by a special code.<br>
Events have there own `Event Codes`.<Br>
Requests and Responses share `Operation Codes`.<br>

Every packet is packed like a dictionary or table, or more simple; key/value pairs.<br>
The data in the packet has a key, which is a single byte.<br>
The data itself is given to us as a plain object, which we must convert to the actual value type.<br>

Every packet has a key of 252 or 253, which tells us the code that identifies the packet.<br>
252 are events<br>
253 are operations<br>
So, if a packet has 252=63, this is the Chat Message event.<br>

Requests and Responses have a special key, 255, that pairs a Response to a Request.<br>
For example, the client sends a Request where 255 = 10.<br>
Eventually the client will receive a Response where 255 = 10.<br>
Also, they both will have the same code on the 253 key.<br>

Finally, Albion uses the Photon Engine for networking.<br>
Luckily for us, people have already made libraries to decode Photon's packets.<br>
If you're interested in seeing how that works, check out any of these:<br>
- [`C# PhotonPacketParser`](https://github.com/0blu/PhotonPackageParser)
- [`Go PhotonSpectator`](https://github.com/ao-data/photon-spectator)

To find alot more Albion related projects, visit [`AO-Data`](https://github.com/ao-data).

## License
See [`LICENSE`](LICENSE).<br>
You are free to use this code however you want. However, you are not allowed to redistribute the code unless your code is also open source. The nature of this project is delicate, since it looks into the user's network stream, so we need to take special care to be as transparent as possible. Any closed source projects of this nature are in direct violation of this transparency, and therefore should not be trusted.
