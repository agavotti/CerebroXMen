- Repositorio github:
	https://github.com/agavotti/CerebroXMen

- Modo de ejecuccion:
	Request a http://cerebroxmenapi.azurewebsites.net/mutant con metodo POST.
	en body enviar el json del DNA buscsando.
	ejemplo:
	{
		 "dna":["CCACTA",
				"CAGGTC",
				"TTCTGT",
				"AGTAGG",
				"ATGCAG",
				"TCACTG"]
	}

	Para llamar al servicio stats hacer la siguiente request: 
	http://cerebroxmenapi.azurewebsites.net/stats

- API alojada en azure con limitaciones de alojamiento gratuito.
	http://cerebroxmenapi.azurewebsites.net
