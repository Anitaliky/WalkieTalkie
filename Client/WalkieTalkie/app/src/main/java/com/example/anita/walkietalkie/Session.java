package com.example.anita.walkietalkie;

/*Connection to server class*/

import android.app.Activity;
import android.os.Handler;
import android.view.View;

import java.io.BufferedWriter;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

public class Session {
    private static Session instance;
    private static final String address = "192.168.43.246";
    private static final int port = 4242;
    private Socket socket;

    public static Session getInstance(final Activity activity, final Handler handler) {
        if (instance == null)
            instance = new Session(activity, handler);
        return instance;
    }

    private Session(final Activity activity, final Handler handler) {
        try {
            socket = new Socket(address, port);
        } catch (Exception e) {
            e.printStackTrace();
        }
        new Thread(new Runnable() {
            @Override
            public void run() {
                while (true) {
                    try {
                        DataInputStream packet = new DataInputStream(socket.getInputStream());
                        System.out.println("message from server!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        byte op = packet.readByte();
                        ServerOperation opcode = ServerOperation.DEFAULT;
                        for (ServerOperation operation : ServerOperation.values())
                            if (op == operation.getValue()) {
                                opcode = operation;
                                break;
                            }
                        if (opcode == ServerOperation.DEFAULT)
                            System.out.println("Unhandled operation " + op);
                        opcode.handle(packet, activity, handler);
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
            }
        }).start();
    }

    public void Send(OutPacket packet) throws IOException {
        socket.getOutputStream().write(packet.toByteArray());
    }
    
    public void SignIn(String username, String password) throws IOException {
        try (OutPacket packet = new OutPacket(ClientOperation.SIGNIN)) {
            packet.writeString(username);
            packet.writeString(password);
            Send(packet);
        }
        System.out.println("Sign in packet!!!!!!!!!!!!!!!");
    }

    public void SignUp(String username, String password) throws IOException {
        try (OutPacket packet = new OutPacket(ClientOperation.SIGNUP)) {
            packet.writeString(username);
            packet.writeString(password);
            Send(packet);
        }
    }
}
