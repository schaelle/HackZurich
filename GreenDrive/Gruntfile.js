/// <binding BeforeBuild='default' ProjectOpened='watch' />
module.exports = function (grunt) {

    grunt.registerTask('firebase-deploy',
        "Firebase deployment",
        function (target) {
            var done = this.async();

            grunt.config.requires('firebase-deploy.config.token');
            grunt.config.requires('firebase-deploy.'+target+'.project');
            grunt.config.requires('firebase-deploy.'+target+'.cwd');
            var client = require('firebase-tools');

            var config = {
              token:grunt.config('firebase-deploy.config.token'),
              project:grunt.config('firebase-deploy.'+target+'.project'),
              cwd:grunt.config('firebase-deploy.'+target+'.cwd')
            };

            client.deploy(config).then(function (data) {
                done();
            }).catch(function (err) {
                grunt.log.writeln(err);
                done(false);
            });
        });

    grunt.initConfig({
        config: {
            environment: 'dev',
            firebase: {
                dev: {
                    apiKey: "AIzaSyB65mdNAnghihSJSBXaeR51P5XBaueSxww",
                    authDomain: "powwow-dev.firebaseapp.com",
                    databaseURL: "https://powwow-dev.firebaseio.com",
                    storageBucket: "powwow-dev.appspot.com"
                },
                pd: {
                    apiKey: "AIzaSyCUjiSWCHO5ZT0YvtG8Uiq3XDrIcfQtXkM",
                    authDomain: "powwow-rch.firebaseapp.com",
                    databaseURL: "https://powwow-rch.firebaseio.com",
                    storageBucket: "powwow-rch.appspot.com"
                }
            },
            firebaseKey:"1/iHSIf-nUCAeFpx3sOlVSqtDtoiagkxjdEWDQRGwmM2A"
        },
        "firebase-deploy":{
          config:{
            token:"<%= config.firebaseKey %>"
          },
          dev:{
            project:"dev",
            cwd:"public"
          },
          pd:{
            project:"pd",
            cwd:"public"
          }
        },
        uglify: {
            options: {
                compress: false,
                mangle: false
            },
            app: {
                files: {
                    'dest/output.js': [require('wiredep')().js, 'dest/app.js']
                }
            }
        },
        wiredep: {
            task: {
                // Point to the files that should be updated when
                // you run `grunt wiredep`
                src: [
				'index.html',
				//'app/views/**/*.html',   // .html support...
				//'app/views/**/*.jade',   // .jade support...
				//'app/styles/main.scss',  // .scss & .sass support...
				//'app/config.yml'         // and .yml & .yaml support out of the box!
                ],

                options: {
                    // See wiredep's configuration documentation for the options
                    // you may pass:

                    // https://github.com/taptapship/wiredep#configuration
                }
            }
        },
        watch: {
            app: {
                files: ['app/**/*', 'views/**/*', 'index.html'],
                tasks: [],
                options: {
                    atBegin: true,
                    livereload: true
                }
            },
            bower: {
                files: ['bower.json'],
                tasks: ['wiredep'],
                options: {
                    livereload: true
                }
            }
        },

        clean: {
            firebase: ['public']
        },
        copy: {
            firebase: {
                files: [
					          { expand: true, src: ['dest/output*', 'dest/config.js', 'views/*', 'app/**/*.html', 'assets/*', 'favicon.ico'], dest: 'public/', filter: 'isFile' },
                    { expand: true, cwd:'bower_components/font-awesome/fonts', src: ['**/*'], dest: 'public/fonts/', filter: 'isFile' }
                ]
            },
            uglify:{
              files: [
                  { expand: true, src: ['dest/output*', 'dest/config.js', 'views/*', 'app/**/*.html', 'assets/*', 'favicon.ico'], dest: 'public/', filter: 'isFile' }
              ]
            }
        },
        cssmin: {
            minify: {
                files: { 'dest/output.css': [require('wiredep')().css, 'app/styles/**/*.css'] }
            }
        },
        usemin: {
            html: 'public/index.html'
        },
        processhtml: {
            options: {
                // Task-specific options go here.
            },
            dist: {
                files: { 'public/index.html': ['index.html'] }
            },
        },
        exec: {
            firebase: 'firebase use "<%= config.environment %>" --token "<%= config.firebaseKey %>" && firebase deploy --token "<%= config.firebaseKey %>"'
        },
        cachebreaker: {
            dist: {
                options: {
                    match: [
                        {
                            'dest/output.css': 'public/dest/output.css',
                            'dest/output.js': 'public/dest/output.js'
                        }
                    ],
                    replacement: 'md5'
                },
                files: {
                    src: ['public/index.html']
                }
            }
        },
        concat: {
            js: {
                src: ['app/app.js', 'app/**/*.js'],
                dest: 'dest/app.js'
            },
            uglify:{
               src: [require('wiredep')().js, 'dest/app.js'],
               dest:'dest/output.js'
            }
        },
        ngAnnotate: {
            options: {
                // Task-specific options go here.
            },
            dist: {
                options: {
                    singleQuotes: true
                },
                files: {
                    'dest/app.js': 'dest/app.js'
                }
            },
            remove: {
                options: {
                    remove: true,
                    add: false
                },
                files: [{
                    expand: true,
                    src: ['app/**/*.js']
                }]
            }
        },
        prompt: {
            environment: {
                options: {
                    questions: [
          						{
          						    config: 'config.environment', // arbitrary name or config for any other grunt task
          						    type: 'list', // list, checkbox, confirm, input, password
          						    message: 'Deployment envirnoment', // Question to ask the user, function needs to return a string,
          						    default: 'dev', // default value if nothing is entered
          						    choices: ["dev", "pd"]
          						}
                    ]
                }
            }
        },
        replace: {
            firebase: {
                options: {
                    patterns: [
                        {
                            match: 'environmentConfig',
                            replacement: '<%= JSON.stringify(config.firebase[config.environment]) %>'
                        }
                    ]
                },
                files: [
                  { expand: true, flatten: true, src: ['dest/config.js'], dest: 'dest/' }
                ]
            }
        },
        "file-creator": {
            "config": {
                "dest/config.js": function (fs, fd, done) {
                    fs.writeSync(fd, '<script>// this file is auto-generated.  DO NOT MODIFY\n');
                    fs.writeSync(fd, '// Initialize Firebase\r\n' +
                        'var config = @@environmentConfig;\r\n' +
                        'firebase.initializeApp(config);\r\n' +
                        '</script>');
                    done();
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-wiredep');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-usemin');
    grunt.loadNpmTasks('grunt-exec');
    grunt.loadNpmTasks('grunt-cache-breaker');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-ng-annotate');
    grunt.loadNpmTasks('grunt-prompt');
    grunt.loadNpmTasks('grunt-replace');
    grunt.loadNpmTasks('grunt-file-creator');
    grunt.loadNpmTasks('grunt-processhtml');

    grunt.registerTask('default', ['wiredep']);
    grunt.registerTask('dist', ['wiredep', 'concat:js', 'ngAnnotate:dist', 'concat:uglify', 'cssmin:minify', 'clean:firebase', 'firebase-config', 'copy:firebase', 'processhtml:dist', 'usemin', 'cachebreaker:dist']);
    grunt.registerTask('upload', ['prompt:environment', 'dist', 'firebase-deploy:dev']);
    grunt.registerTask('upload-build', ['dist', 'firebase-deploy:dev']);
    grunt.registerTask('upload-build-pd', ['dist', 'firebase-deploy:pd']);
    grunt.registerTask('firebase-config', ['file-creator:config', 'replace:firebase']);
};
