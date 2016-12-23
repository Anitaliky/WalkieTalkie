package com.example.anita.walkietalkie;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStreamReader;
import java.net.Socket;

public class SignIn extends Activity implements OnClickListener {
    Button signInButton, signUpButton;
    EditText userName, password;
    String address = "0.0.0.0", port = "4242";

    public enum ClientMessageType{
        SIGNIN("100"),
        SIGNUP("101");
        private final String value;
        private ClientMessageType(final String value) {this.value = value;}
        public String toString() {return value;}
        }

    public enum ServerMessageType{
        SIGNIN("201"),
        SIGNUP("203");
        private final String value;
        private ServerMessageType(final String value) { this.value = value; }
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
                //TODO - connect to server
                new Thread(new Runnable() {
                    public void run() {
                        try {
                            //client
                            String message;
                            String modifiedSentence;
                            //BufferedReader inFromUser = new BufferedReader( new InputStreamReader(System.in));
                            Socket clientSocket = new Socket(address, Integer.parseInt(port));
                            DataOutputStream outToServer = new DataOutputStream(clientSocket.getOutputStream());
                            BufferedReader inFromServer = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
                            while(true){
                                //sentence = inFromUser.readLine();
                                message = createMessageToSend(v, ClientMessageType.SIGNIN.toString());
                                outToServer.writeBytes(message);
                                modifiedSentence = inFromServer.readLine();
                                System.out.println("FROM SERVER: " + modifiedSentence);
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


    private String createMessageToSend(View v, final String messageType){
        String message = "";
        switch (messageType){
            case ClientMessageType.SIGNIN.toString():
                message += ClientMessageType.SIGNIN.toString();
                break;

        }
        return message;
    }
}
