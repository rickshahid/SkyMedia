pipeline {
  agent any
  stages {
    stage('Apply') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply -input=false tfplan
'''
      }
    }

  }
}