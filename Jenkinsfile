node('vm1') {
	echo sh('printenv | sort')

	stage 'checkout Source Code'
		checkout scm
		
	stage 'Build aplikasi'
		sh "printenv > .env"
		sh "docker compose --env-file .env -f docker-compose.yml up  -d --build"
}
