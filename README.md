# Yondulib Project
## El proyecto tras el _Unity Package_: [```yondulib```](https://github.com/nubango/yondulib)

Yondulib es un plugin para Unity que permite utilizar sonidos detectados por el micrófono de un ordenador como entradas (controles) en videojuegos. Para descargar e instalar el plugin hay que comprobar los requisitos y seguir los pasos descritos en el [repositorio del plugin](https://github.com/nubango/yondulib).

### Estructura de reconocedores 

El sistema de reconocimiento se compone de tres ficheros: _SoundRecognizer_, _ClickRecognizer_ y _WhistleRecognizer_. _SoundRecognizer_ es la clase base de la que heredan las otras dos clases. Las clases hijas se encargan de analizar el audio y evaluar el grado de coincidencia a través del método _AnalizeSpectrum()_. _SoundRecognizer_ se encarga de generar el evento correspondiente si el grado de coincidencia es lo suficientemente alto.

![Estructura de los reconocedores](/Images/Recognizers.png)

Para más imagenes explicativas sobre la estructura del proyecto, ver la carpeta [Images](/Images).

### Información adicional

Este repositorio forma parte del Trabajo de Fin de Grado: "Yondulib: Herramienta para el uso de sonidos como método de control de videojuegos Unity".

Creado por Gonzalo Alba Durán y Nuria Bango Iglesias para la Universidad Complutense de Madrid, dirigido por Manuel Freire Morán.
