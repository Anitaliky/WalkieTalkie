package com.example.anita.walkietalkie;

import java.io.Closeable;
import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;

//TODO
public class InPacket implements Closeable {
    private DataInputStream m_arrayIn;
    private Socket m_socket;
    public InPacket(Socket socket) throws IOException {
        m_socket = socket;
        m_arrayIn = new DataInputStream(m_socket.getInputStream());
    }

    @Override
    public void close() throws IOException {
        m_arrayIn.close();
    }
}
