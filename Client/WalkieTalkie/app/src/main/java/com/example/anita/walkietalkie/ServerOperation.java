package com.example.anita.walkietalkie;


import android.app.Activity;
import android.os.Handler;
import android.view.View;
import android.widget.TextView;

import java.io.DataInputStream;
import java.io.IOException;



public enum ServerOperation {

    SIGNIN(0) {
        @Override
        public void handle(DataInputStream packet, final Activity activity, final Handler handler) throws IOException {
            final byte result = packet.readByte();
            //do it every time
            handler.post(new Runnable() {
                @Override
                public void run() {
                    TextView messageView = (TextView) activity.findViewById(R.id.messageView);
                    System.out.println("signin answer: " + result);
                    switch (result) {
                        case 0: //success
                            messageView.setText
                                    ("Successful connected!");
                            break;
                        case 1: //user doesn’t exist
                            messageView.setText
                                    ("This user doesn’t exist. Enter an exist one or sign up");
                            break;
                        case 2: //wrong details
                            messageView.setText
                                    ("Username or password doesn't match. Try again");
                            break;
                        case 3: //user is already connected
                            messageView.setText
                                    ("This user is already connected");
                            break;
                        case 7: //other
                            messageView.setText
                                    ("something went wrong");
                            break;
                    }
                }
            });
        }
    },
    SIGNUP(1) {
        @Override
        public void handle(DataInputStream packet, final Activity activity, final Handler handler) throws IOException {
            final byte result = packet.readByte();
            //do it every time
            handler.post(new Runnable() {
                @Override
                public void run() {
                    TextView messageView = (TextView) activity.findViewById(R.id.messageView);
                    switch (result) {
                        case 0: //success
                            messageView.setText
                                    ("Successful connected!");
                            break;
                        case 4: //Invalid username
                            messageView.setText
                                    ("Invalid username");
                            break;
                        case 5: //Username is already exists
                            messageView.setText
                                    ("Username is already exists. Try again");
                            break;
                        case 6: //invalid password
                            messageView.setText
                                    ("invalid password");
                            break;
                        case 7: //other
                            messageView.setText
                                    ("something went wrong");
                            break;
                    }
                }
            });
        }
    },
    DEFAULT(-1) {
        @Override
        public void handle(DataInputStream packet, final Activity activity, final Handler handler) {
            TextView messageView = (TextView) activity.findViewById(R.id.messageView);
            messageView.setText("Unhandled operation");
        }
    }
    ;

    private byte value;

    private ServerOperation(int value) {
        this.value = (byte) value;
    }

    public byte getValue() {
        return value;
    }

    public abstract void handle(DataInputStream packet, final Activity activity, final Handler handler) throws IOException;
}