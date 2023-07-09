from os import path, chdir, listdir, rename
path = path.abspath(__file__)
chdir(path.split('Rename.py')[0])


# songs = listdir('Chill\lyrical\Tier2')
# chdir('Chill\lyrical\Tier2')
# for song in songs:
#     dst = f'{song[7:]}'
#     src = f'{song}'
#     rename(src,dst)
#folders = listdir('Chill\lyrical')
folders = listdir("Chill/NoLyrics")
chdir ("Chill/NoLyrics")
for folder in folders:
    songs = listdir(folder)
    for song in songs:
            songn = song.replace('-', ' ')
            dst = f'{folder}/{folder} - {songn}'
            src = f'{folder}/{song}'
            rename(src,dst)