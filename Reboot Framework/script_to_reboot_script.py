import os
from xml.dom.minidom import *

xmlDocument = Document()
root = xmlDocument.createElement("Reboot")

for file in os.listdir("."):
    if ".py" not in file and ".xml" not in file:
        child = xmlDocument.createElement("AutoAssembly")
        child.setAttribute('Name', file)
        root.appendChild(child)

        r = open(file, 'r')
        child.appendChild(xmlDocument.createTextNode(r.read()))
        r.close()

xmlDocument.appendChild(root)

f = open('Script.xml', 'w')
xmlDocument.writexml(f, "    ", "    ", '\n')
xmlDocument.unlink()
f.close()
