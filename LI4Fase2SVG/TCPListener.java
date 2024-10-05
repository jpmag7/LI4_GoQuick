import java.net.*;
import java.io.*;

public class TCPListener{

	private static ServerSocket socket;

	public static void main(String[] args) throws Exception{
		socket = new ServerSocket(12345);
		run();
	}

	public static void run(){
		try{
			Socket s = socket.accept();

			BufferedReader in = new BufferedReader(new InputStreamReader(s.getInputStream()));
			DataOutputStream out = new DataOutputStream(new BufferedOutputStream(s.getOutputStream()));

			in.read(new char[3], 0, 3);

			// Para ler (usar readLine)
			System.out.println(in.readLine());
			System.out.println(in.readLine());

			// Para enviar (enviar em bytes)
			out.write((45.45475475 + "\n").getBytes());
			out.flush();


		}catch(Exception e){e.printStackTrace();}
	}
}