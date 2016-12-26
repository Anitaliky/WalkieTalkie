package com.example.anita.walkietalkie;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Switch;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStreamReader;
import java.net.Socket;

public class SignIn extends Activity implements OnClickListener {
    Button signInButton, signUpButton;
    EditText userName, password;
    String address = "0.0.0.0", port = "4242";

    public enum ClientMessageType{
        SIGNIN(100),
        SIGNUP(101);
        private int value; private ClientMessageType(int value) { this.value = value; }
        }

    public enum ServerMessageType{
        SIGNIN(201),
        SIGNUP(203);
        private int value; private ServerMessageType(int value) { this.value = value; }
    }

    public enum ServerResponseType{
        SUCCESS("0"),
        WRONG_DETAILS("2"),
        ALREADY_CONNECTED("3"),
        INVALID_USENAME("4"),
        ALREADY_EXISTS("5"),
        INAVALID_PASSWORD("6"),
        OTHER("7");
        private final String value;
        private ServerResponseType(final String value) {this.value = value;}
        public String toString() {return value;}
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sign_in);

        signInButton = (Button) findViewById(R.id.signIn);
        signUpButton = (Button) findViewById(R.id.signUp);
        userName = (EditText) findViewById(R.id.username);
        password = (EditText) findViewById(R.id.password);
        signUpButton.setOnClickListener(this);
        signInButton.setOnClickListener(this);
    }

    @Override
    public void onClick(final View v) {
        // detect the view that was "clicked"
        switch(v.getId()){
            case R.id.signIn:
                new Thread(new Runnable() {
                    public void run() {
                        try {
                            //client
                            String request;
                            String response;
                            //BufferedReader inFromUser = new BufferedReader( new InputStreamReader(System.in));
                            Socket clientSocket = new Socket(address, Integer.parseInt(port));
                            DataOutputStream outToServer = new DataOutputStream(clientSocket.getOutputStream());
                            BufferedReader inFromServer = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
                            while(true){
                                //sentence = inFromUser.readLine();
                                request = createMessageToSend(ClientMessageType.SIGNIN.value);
                                outToServer.writeBytes(request);
                                response = inFromServer.readLine();
                                parseMessage(response);
                                System.out.println("FROM SERVER: " + response);
                            }
                        } catch (Exception e){
                            System.out.println("FROM SERVER: " + e.toString());
                        }
                    }
                }).start();
                break;

            case R.id.signUp:
                //TODO - connect to server

                break;
        }

    }


    private String createMessageToSend(int messageType){
        String message = "";
        switch (messageType){
            case ClientMessageType.SIGNIN.value:
                message += ClientMessageType.SIGNIN.toString(); //add message type
                String usernameToSend = userName.getText().toString(),
                        passwordToSend = password.getText().toString();

                message += usernameToSend.length(); //username len
                message += usernameToSend;
                message += passwordToSend.length(); //pass len
                message += passwordToSend;
                break;

            case ClientMessageType.SIGNUP.value:
                message += ClientMessageType.SIGNUP.toString();
                usernameToSend = userName.getText().toString();
                passwordToSend = password.getText().toString();

                message += usernameToSend.length(); //username len
                message += usernameToSend;
                message += passwordToSend.length(); //pass len
                message += passwordToSend;
                break;

        }
        return message;
    }

    private void parseMessage(String message){
        switch (message.substring(0,3)){
            case ServerMessageType.SIGNIN.value:

                break;

            case ServerMessageType.SIGNUP.toString():

                break;
        }
    }
}
