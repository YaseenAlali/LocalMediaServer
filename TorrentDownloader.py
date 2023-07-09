from qbittorrent import Client
from sys import argv
from os import path, chdir

qb = Client('http://192.168.1.2:50005/')
qb.login('admin', 'listen')
path = path.abspath(__file__)
chdir(path.split('TorrentDownloader.py')[0])


def download(link):
	qb.download_from_link(link)


link = ""
with open("torrentLink.txt", 'r') as f:
	link = f.read()

download(link)
	