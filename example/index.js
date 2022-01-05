// This example shows us how to use the sniffer program.
// First build the sniffer, then move the exe file next to this file.
// Run the script with node, and it should work.
// A more advanced approach can be seen in the tcc-client.

const net = require('net');
const spawn = require('child_process').spawn;
const path = require('path');



// Spawning the sniffer, and closing it on exit.
const snifferProcess = spawn(
    path.join(__dirname, '/sniffer.exe'),
    { stdio: 'inherit' }
);

process.on('exit', (code) =>
{
    snifferProcess.kill();
});



// Connecting to the sniffer server.
const socket = net.connect(9999, '127.0.0.1');
let socketBuffer = '';

socket.on('connect', () =>
{
    // We want encoding as utf8, since some data comes in as utf8.
    socket.setEncoding('utf8');
});

socket.on('data', (data) =>
{
    // Data is sent as JSON strings.
    // Since we use TCP, data comes in as a stream.
    // All this means is the data can be fragmented between multiple messages.
    // To get around this, add the data into a buffer.
    socketBuffer += data;

    // Now we need to know when 1 JSON object begins and another ends.
    // For this, we just separate them with new line characters.
    // When we find a new line character, this tells us there is a full JSON object in the buffer.
    // If we find no new line, then we don't need to continue.
    const index = socketBuffer.indexOf('\n');
    if (index == -1) return;

    // Now that we found a JSON object, lets extract and remove it from the buffer.
    const msg = socketBuffer.slice(0, index + 1);
    socketBuffer = socketBuffer.replace(msg, '');

    // We have the object, so now just convert it to an actual javascript object, and do whatever with it.
    // In this example, we just print the data.
    console.log(JSON.parse(msg));
});

socket.on('error', (error) =>
{
    // We are required to capture errors.
    console.log(error);
});