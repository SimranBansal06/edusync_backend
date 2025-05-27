# Initialize Git repository and push to GitHub
Write-Host "Initializing Git repository..."

# Check if .git directory already exists
if (-not (Test-Path -Path ".git")) {
    git init
}

# Check if remote already exists
$remoteExists = git remote -v | Select-String -Pattern "origin" -Quiet
if (-not $remoteExists) {
    Write-Host "Adding remote origin..."
    git remote add origin https://github.com/Simranbansal03/edusync_backend.git
}

# Add all files
Write-Host "Adding files to Git..."
git add .

# Make initial commit
Write-Host "Making initial commit..."
git commit -m "Initial commit of EduSync backend API"

# Push to GitHub
Write-Host "Pushing to GitHub..."
Write-Host "Note: You'll need to authenticate with your GitHub credentials"
git push -u origin main

Write-Host "Done! Your code is now on GitHub at: https://github.com/Simranbansal03/edusync_backend.git" 