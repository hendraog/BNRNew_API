node(env.NODE) {
	echo sh('printenv | sort')

	stage 'checkout Source Code'
		checkout scm
		
	stage 'Build aplikasi'
		sh "printenv > .env"
		sh "docker compose --env-file .env -f PosFnbStack.yml up  -d --build"
}
