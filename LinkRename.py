import os
import winshell

def modify_shortcuts(directory: str, old_path: str, new_path: str) -> None:
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith(".lnk"):
                shortcut_path = os.path.join(root, file)
                shortcut = winshell.shortcut(shortcut_path)
                if old_path in shortcut.path:
                    shortcut.path = shortcut.path.replace(old_path, new_path)
                    shortcut.description="new"
                    print(f"Modified: {shortcut_path}")
                    shortcut.write()


def modify_shortcut(path: str, old_path: str, new_path: str) -> None:
    if path.endswith(".lnk"):
        shortcut_path = path
        shortcut = winshell.shortcut(shortcut_path)
        print(f"{shortcut.path}\n")
        if old_path in shortcut.path:
            shortcut.path = shortcut.path.replace(old_path, new_path)
            shortcut.description="new"
            print(f"Modified: {shortcut_path} to {shortcut.path}\nworking directory -> {shortcut.working_directory}")
            shortcut.write()
        if old_path in shortcut.working_directory:
            shortcut.working_directory=shortcut.working_directory.replace(old_path,new_path)
            print(f"working directory changed {shortcut.working_directory}\n")
            shortcut.write()
    else:
        print("no link provided")

if __name__ == "__main__":
    directory = r"D:\NextCloud\400_Programmierung\Totlagenkonstruktion\oldLinks\Vorbereitung_Uebung_6-Beschleunigungen.pptx - Verknüpfung.lnk"
    old_path = r"Y:"
    new_path = r"\\afs\.tu-chemnitz.de\project\mht_lehre"
    modify_shortcut(directory, old_path, new_path)

