from flask import Flask
import geopy.distance
import os
from flaskext.mysql import MySQL

app = Flask('GoQuickServer')
mysql = MySQL()
app.config['MYSQL_DATABASE_HOST'] = os.environ['host']
app.config['MYSQL_DATABASE_USER'] = os.environ['user']
app.config['MYSQL_DATABASE_PASSWORD'] = os.environ['pass']
app.config['MYSQL_DATABASE_DB'] = os.environ['name']
mysql.init_app(app)


def run():
  app.run(host='0.0.0.0', port=8080)


@app.route('/')
def home():
    return "Online"


@app.route('/request/<payload>')
def request(payload):
  data = payload.split("«")
  if data[0] != os.environ['access_key']:
    return "Wrong access_key"
  opcao = data[1]
  conn = mysql.connect()
  cursor = conn.cursor()
  if opcao == "1":
    coord1 = (float(data[2]), float(data[3]))
    raio = float(data[4])
    cursor.execute("SELECT nome, latitude, longitude FROM paragem")
    rows = cursor.fetchall()
    res = ""
    for row in rows:
        coord2 = (row[1], row[2])
        dist = geopy.distance.distance(coord1, coord2).m
        if dist < raio:
            res = res + row[0] + "«" + str(row[1]) + "«" + str(
                row[2]) + "«"
    cursor.close()
    return res
  elif opcao == "2":
      cursor.execute("SELECT * FROM paragem WHERE nome='" + data[2] + "'")
      paragem = cursor.fetchone()
      if paragem == None:
        return "error"
      res = ""
      for s in paragem:
          res = res + str(s) + "«"
      cursor.execute(
          "SELECT id_rota FROM paragem_has_rota WHERE id_paragem=" +
          str(paragem[0]))
      ids_rotas = cursor.fetchall()
      for id in ids_rotas:
          cursor.execute("SELECT * FROM rota WHERE id_rota=" + str(id[0]))
          rowsrotas = cursor.fetchall()
          for rota in rowsrotas:
              res = res + str(rota[0]) + "«"
              cursor.execute("SELECT nome FROM paragem WHERE id_Paragem=" +
                             str(rota[1]))
              nomeOrigem = cursor.fetchone()
              res = res + str(nomeOrigem[0]) + "«"
              cursor.execute("SELECT nome FROM paragem WHERE id_Paragem=" +
                             str(rota[2]))
              nomeDestino = cursor.fetchone()
              res = res + str(nomeDestino[0]) + "«" + str(rota[3]) + "«" + str(rota[
                  4]) + "«" + str(rota[5]) + "«" + str(rota[6]) + "«" + str(rota[7]) + "«"
      cursor.close()
      return res
  else:
      cursor.close()
      return "error"


run()
