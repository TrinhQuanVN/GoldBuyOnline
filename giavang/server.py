import http.server
import socketserver
import socket

PORT = 8000

Handler = http.server.SimpleHTTPRequestHandler

with socketserver.TCPServer(("", PORT), Handler) as httpd:
    print(f"Serving at http://{socket.gethostbyname(socket.gethostname())}:{PORT}")
    httpd.serve_forever()
