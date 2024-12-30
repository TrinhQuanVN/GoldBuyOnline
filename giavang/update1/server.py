import http.server
import socketserver
import socket
import json

PORT = 8000

# Data storage for gold prices (using float for numeric operations)
gold_prices = [
    {"type": "24k", "buy": 8280000.0, "sell": 8400000.0},
    {"type": "23k", "buy": 8250000.0, "sell": 8390000.0},
    {"type": "18k", "buy": 6100000.0, "sell": 7100000.0},
    {"type": "16k", "buy": 4900000.0, "sell": 5900000.0},
    {"type": "14k", "buy": 4600000.0, "sell": 5600000.0},
    {"type": "10k", "buy": 3300000.0, "sell": 4300000.0}
]

class MyRequestHandler(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/gold-prices":
            # Format the prices with commas for the response
            formatted_prices = [
                {**price, "buy": f"{price['buy']:,.0f}", "sell": f"{price['sell']:,.0f}"}
                for price in gold_prices
            ]
            self.send_response(200)
            self.send_header("Content-Type", "application/json")
            self.end_headers()
            self.wfile.write(json.dumps(formatted_prices).encode())
        else:
            super().do_GET()

    def do_POST(self):
        if self.path == "/update-price":
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            data = json.loads(post_data)

            # Update the gold_prices list
            for price in gold_prices:
                if price["type"] == data["type"]:
                    price["buy"] = float(data["buy"])
                    price["sell"] = float(data["sell"])
                    break

            self.send_response(200)
            self.end_headers()
            self.wfile.write(b"Updated successfully")

with socketserver.TCPServer(("", PORT), MyRequestHandler) as httpd:
    print(f"Serving at http://{socket.gethostbyname(socket.gethostname())}:{PORT}")
    httpd.serve_forever()
